using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DanubeRS.DBCParser.Parser;
using DanubeRS.DBCParser.Tokenizer;
using Microsoft.Extensions.Logging;
using ValueType = DanubeRS.DBCParser.Parser.ValueType;

namespace DanubeRS.DBCParser;

public class Database(ILogger<Database> logger)
{
    public class SignalDefinition(Message message, Signal signal, ParsedDb db)
    {
        public Signal Signal { get; } = signal;
        public string[] Comments { get; } = db.Comments
            .OfType<SignalComment>()
            .Where(c => c.MessageId == message.Header.Id && c.SignalName == signal.Name)
            .Select(c => c.Comment)
            .ToArray();

        public IReadOnlyDictionary<long, string>? ValueLookup { get; } = db.ValueDescriptions
            .FirstOrDefault(v => v.MessageId == message.Header.Id && v.SignalName == signal.Name)?.Values;
    }

    public class MessageDefinition(Message message, ParsedDb db)
    {
        public readonly SignalDefinition[] Signals =
            message.Signals.Select(s => new SignalDefinition(message, s, db)).ToArray();

        public readonly MessageHeader Header = message.Header;
        public readonly bool IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
        public readonly string[] Comments = db.Comments
            .OfType<MessageComment>()
            .Where(c => c.MessageId == message.Header.Id)
            .Select(c => c.Comment)
            .ToArray();
    }

    private readonly Dictionary<uint, MessageDefinition> _messageDefn = new();
    private readonly byte[] _signalValueBytes = new byte[8];

    /// <summary>
    /// Attributes defined in the file (BA_DEF_ sections).
    /// </summary>
    public ImmutableArray<AttributeDefinition> AttributeDefinitions { get; private set; } = [];

    /// <summary>
    /// Attribute default values from the file (BA_DEF_DEF_ sections).
    /// </summary>
    public ImmutableArray<AttributeDefault> AttributeDefaults { get; private set; } = [];

    /// <summary>
    /// Attribute values assigned to nodes/messages/signals/env-vars (BA_ sections).
    /// </summary>
    public ImmutableArray<AttributeValue> AttributeValues { get; private set; } = [];

    /// <summary>
    /// Comments attached to the database, nodes, messages, signals and env vars (CM_ sections).
    /// </summary>
    public ImmutableArray<CommentBase> Comments { get; private set; } = [];

    /// <summary>
    /// Version string from the file.
    /// </summary>
    public string? Version { get; private set; }

    public void AddFile(TextReader reader)
    {
        var dbcString = reader.ReadToEnd();
        var tokens = DbcTokenizer.Instance.Tokenize(dbcString);
        var parsed = DbcParser.Database(tokens);
        if (!parsed.HasValue)
        {
            var position = parsed.ErrorPosition.Absolute;
            var contextStart = Math.Max(0, position - 40);
            var context = dbcString[contextStart..Math.Min(position + 60, dbcString.Length)]
                .Replace("\r", "").Replace("\n", "⏎");
            var expectations = parsed.FormatErrorMessageFragment();
            throw new FormatException(
                $"Failed to parse DBC file at position {position} (line {parsed.ErrorPosition.Line}, column {parsed.ErrorPosition.Column}): expected {expectations}. Context: ...{context}...");
        }

        StoreParsedResult(parsed.Value);
    }

    private void StoreParsedResult(ParsedDb parsed)
    {
        // Multiple files accumulate: attribute definitions of the same name are de-duplicated,
        // later values win; comments and values are appended.
        var defnByKey = AttributeDefinitions
            .Select(d => (Key: (d.ObjectType ?? "", d.Name), Defn: d))
            .ToDictionary(x => x.Key, x => x.Defn);
        foreach (var d in parsed.AttributeDefinitions)
            defnByKey[(d.ObjectType ?? "", d.Name)] = d;
        AttributeDefinitions = defnByKey.Values.ToImmutableArray();

        var defByKey = AttributeDefaults
            .Select(d => (d.AttributeName, d))
            .ToDictionary(x => x.AttributeName, x => x.d);
        foreach (var d in parsed.AttributeDefaults)
            defByKey[d.AttributeName] = d;
        AttributeDefaults = defByKey.Values.ToImmutableArray();

        AttributeValues = AttributeValues.AddRange(parsed.AttributeValues);
        Comments = Comments.AddRange(parsed.Comments);
        Version ??= parsed.Version.VersionString;

        foreach (var message in parsed.Messages)
        {
            _messageDefn.TryAdd((uint)message.Header.Id, new MessageDefinition(message, parsed));
        }
    }

    public bool TryParseBinaryMessage(uint frameId, byte[] data, [NotNullWhen(true)] out MessageValue? value,
        [NotNullWhen(true)] out MessageDefinition? defn)
    {
        value = null;
        if (!_messageDefn.TryGetValue((uint)frameId, out defn))
        {
            return false;
        }

        var bitArray = new BitArray(data);
        if (logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace("Data\n{BitArrayData}", GetBitArrayDataForLog(bitArray));

        return defn.IsMultiplexed switch
        {
            true => TryParseMultiplexedMessage(defn, bitArray, out value),
            false => TryParseRegularMessage(defn, bitArray, out value)
        };
    }

    private bool TryParseRegularMessage(MessageDefinition defn, BitArray data, out MessageValue? value)
    {
        value = null;
        if (defn.Signals.Length == 0)
        {
            // Known message without signals: succeed with an empty value rather than
            // returning true with a null value (violating the [NotNullWhen(true)] contract).
            value = new MessageValue(defn.Header.Id, defn.Header.Name, [], null);
            return true;
        }
        var lastSignal = defn.Signals.OrderByDescending(s => s.Signal.StartBit).First();
        // Message size is incompatable
        if (lastSignal.Signal.StartBit + lastSignal.Signal.Size > data.Length) return false;

        // Signals are already ordered by start bit at parse time — no per-frame re-sort.
        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            defn.Signals.Select(signal => GetSignalValue(data, signal)).ToArray(),
            null);
        return true;
    }

    private SignalValue GetSignalValue(BitArray data, SignalDefinition signal)
    {
        Array.Clear(_signalValueBytes);
        BitsToBytes(data, signal.Signal.StartBit, signal.Signal.Size, _signalValueBytes);

        double value;
        if (signal.Signal.ValueType == ValueType.Signed)
        {
            var rawValue = BitConverter.ToInt64(_signalValueBytes, 0);
            rawValue = rawValue >> (signal.Signal.Size - 1) == 0
                ? rawValue
                : -1 ^ CreateBitMask(0, signal.Signal.Size) | rawValue;
            value = signal.Signal.Offset + rawValue * signal.Signal.Factor;
        }
        else
        {
            // Unsigned: compute in floating point so full 64-bit range (max 2^64-1) is preserved
            // instead of wrapping to negative via the long cast.
            var rawValue = BitConverter.ToUInt64(_signalValueBytes, 0);
            value = signal.Signal.Offset + rawValue * (double)signal.Signal.Factor;
        }

        return new SignalValue(signal.Signal.Name, value);
    }

    private static long CreateBitMask(int start, int length)
    {
        ulong mask = 0xffffffffffffffff;
        mask >>= 64 - length;
        mask <<= start;
        return (long)mask;
    }

    private void BitsToBytes(BitArray bitArray, int offset, int length, byte[] bytes)
    {
        int byteIdx = 0,
            bitIdx = 0;

        for (var i = 0; i < length; i++)
        {
            if (i < length)
            {
                if (bitArray[offset + i])
                    bytes[byteIdx] |= (byte)(1 << bitIdx);
            }

            bitIdx++;
            if (bitIdx != 8) continue;
            bitIdx = 0;
            byteIdx++;
        }
    }

    private bool TryParseMultiplexedMessage(MessageDefinition defn, BitArray data, out MessageValue? value)
    {
        value = default;

        var multiplexSignal = defn.Signals.Single(s => s.Signal.Multiplex is { IsSwitch: true });

        var switchBytes = new byte[2];
        BitsToBytes(data, multiplexSignal.Signal.StartBit, multiplexSignal.Signal.Size, switchBytes);
        var switchValue = BitConverter.ToUInt16(switchBytes);
        var filteredSignals = defn.Signals
            .Where(s => s.Signal.Multiplex != null && s.Signal.Multiplex.SwitchValue == switchValue)
            .ToArray();

        var lastSignal = filteredSignals.OrderByDescending(s => s.Signal.StartBit).FirstOrDefault();
        if (lastSignal == null) return false;

        // Message size is incompatible
        if (lastSignal.Signal.StartBit + lastSignal.Signal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            filteredSignals.Select(signal => GetSignalValue(data, signal)).ToArray(), switchValue);

        return true;
    }

    private string GetBitArrayDataForLog(BitArray bitArray)
    {
        var sb = new StringBuilder();
        var len = bitArray.Length;
        var range = Enumerable.Range(0, len).Chunk(8);
        foreach (var bitIdx in range)
        {
            sb.AppendLine(string.Join(' ', bitIdx.Select(i => bitArray.Get(i) ? '1' : '0')));
        }

        return sb.ToString();
    }
}

public record SignalValue(string SignalName, double Value);

public record MessageValue(int MessageId, string MessageName, SignalValue[] Signals, uint? MultiplexValue);