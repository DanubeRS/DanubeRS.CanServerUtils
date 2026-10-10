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

    public readonly record struct SignalBounds(int MinBit, int MaxBit)
    {
        public static SignalBounds For(Signal s)
        {
            if (s.Order == Endianness.Little)
                // Intel: startBit is the LSB, bits ascend contiguously.
                return new SignalBounds(s.StartBit, s.StartBit + s.Size - 1);

            // Motorola sawtooth: bits descend within a byte, then continue at the MSB of the next.
            var minBit = s.StartBit;
            var maxBit = s.StartBit;
            var bitPos = s.StartBit;
            for (var i = 1; i < s.Size; i++)
            {
                bitPos = bitPos % 8 == 0 ? bitPos + 15 : bitPos - 1;
                maxBit = bitPos;
                if (bitPos < minBit) minBit = bitPos;
            }

            return new SignalBounds(minBit, maxBit);
        }

        public bool FitsIn(int totalBits) => MinBit >= 0 && MaxBit < totalBits;
    }

    public class MessageDefinition
    {
        public readonly SignalDefinition[] Signals;
        public readonly MessageHeader Header;
        public readonly bool IsMultiplexed;
        public readonly string[] Comments;

        /// <summary>Endianness-aware bit bounds, index-aligned with <see cref="Signals"/>.</summary>
        public readonly SignalBounds[] SignalBounds;

        /// <summary>Signals that decode for every multiplex value (no m&lt;N&gt; selector), if any.</summary>
        public readonly SignalDefinition[] CommonSignals;

        /// <summary>The multiplexer switch signal (M), when present.</summary>
        public readonly SignalDefinition? MultiplexSignal;

        /// <summary>Grouped m&lt;N&gt; signals by mux value, for O(1) lookup per frame.</summary>
        public readonly IReadOnlyDictionary<uint, SignalDefinition[]> MuxGroups;

        public MessageDefinition(Message message, ParsedDb db)
        {
            Signals = message.Signals.Select(s => new SignalDefinition(message, s, db)).ToArray();
            Header = message.Header;
            IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
            Comments = db.Comments
                .OfType<MessageComment>()
                .Where(c => c.MessageId == message.Header.Id)
                .Select(c => c.Comment)
                .ToArray();

            SignalBounds = Signals.Select(sd => Database.SignalBounds.For(sd.Signal)).ToArray();
            CommonSignals = Signals.Where(sd => sd.Signal.Multiplex is null).ToArray();
            MultiplexSignal = Signals.FirstOrDefault(sd => sd.Signal.Multiplex is { IsSwitch: true });
            MuxGroups = Signals.Where(sd => sd.Signal.Multiplex is { IsSwitch: false })
                .GroupBy(sd => (uint)sd.Signal.Multiplex!.SwitchValue!.Value)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
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
        if (defn.Signals.Length == 0)
        {
            // Known message without signals: succeed with an empty value rather than
            // returning true with a null value (violating the [NotNullWhen(true)] contract).
            value = new MessageValue(defn.Header.Id, defn.Header.Name, [], null);
            return true;
        }

        if (!BoundsFit(defn.Signals, defn.SignalBounds, data.Length))
        {
            value = null;
            return false;
        }

        // Signals are already ordered by start bit at parse time — no per-frame re-sort.
        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            defn.Signals.Select(signal => GetSignalValue(data, signal)).ToArray(),
            null);
        return true;
    }

    private static bool BoundsFit(SignalDefinition[] signals, SignalBounds[] bounds, int totalBits)
    {
        for (var i = 0; i < signals.Length; i++)
        {
            if (!bounds[i].FitsIn(totalBits)) return false;
        }

        return true;
    }

    private SignalValue GetSignalValue(BitArray data, SignalDefinition signal)
    {
        var rawValue = signal.Signal.Order == Endianness.Big
            ? ExtractMotorola(data, signal.Signal.StartBit, signal.Signal.Size)
            : ExtractLittleEndian(data, signal.Signal.StartBit, signal.Signal.Size);

        double value;
        if (signal.Signal.ValueType == ValueType.Signed && signal.Signal.Size <= 64)
        {
            // Arithmetic shift left then right sign-extends the low/high Size bits uniformly
            // for both Intel (LSB-first) and Motorola (MSB-first) raw values.
            var signedRaw = (long)rawValue << (64 - signal.Signal.Size) >> (64 - signal.Signal.Size);
            value = signal.Signal.Offset + signedRaw * signal.Signal.Factor;
        }
        else
        {
            // Unsigned: compute in floating point so full 64-bit range (max 2^64-1) is preserved.
            value = signal.Signal.Offset + (double)rawValue * signal.Signal.Factor;
        }

        return new SignalValue(signal.Signal.Name, value);
    }

    /// <summary>
    /// Little-endian (Intel, '@1') extraction: startBit is the LSB; bits ascend into higher bytes.
    /// Returns the unsigned raw value; Size must be 0..64.
    /// </summary>
    private ulong ExtractLittleEndian(BitArray data, int startBit, int size)
    {
        Array.Clear(_signalValueBytes);
        var byteCount = Math.Min(8, (size + 7) / 8);
        for (var i = 0; i < size; i++)
        {
            var bitIndex = startBit + i;
            if (!data[bitIndex]) continue;
            _signalValueBytes[i / 8] |= (byte)(1 << (i % 8));
        }

        // Clear any stale bytes beyond this signal's span (buffer is shared across signals).
        for (var b = byteCount; b < 8; b++)
            _signalValueBytes[b] = 0;

        return BitConverter.ToUInt64(_signalValueBytes, 0);
    }

    /// <summary>
    /// Big-endian (Motorola, '@0') extraction per the DBC spec: startBit is the MSB position in
    /// LSB0 numbering; subsequent bits descend toward the LSB of the same byte, then continue at
    /// the MSB of the next byte (sawtooth sequence). Returns the raw unsigned value.
    /// </summary>
    private ulong ExtractMotorola(BitArray data, int startBit, int size)
    {
        var bitPos = startBit;
        ulong raw = 0;
        for (var i = 0; i < size; i++)
        {
            if (data[bitPos])
                raw |= 1UL << (size - 1 - i);

            bitPos = bitPos % 8 == 0
                ? bitPos + 15 // reached the LSB of this byte → jump to MSB of the next
                : bitPos - 1;
        }

        return raw;
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
        var multiplexSignal = defn.MultiplexSignal;
        if (multiplexSignal is null)
        {
            value = null;
            return false;
        }

        // Extract the switch value at full width (spec allows mux switches wider than 16 bits).
        var switchValue = (uint)GetSignalValue(data, multiplexSignal).Value;

        var muxGroup = defn.MuxGroups.TryGetValue(switchValue, out var group)
            ? group
            : [];

        // Spec: signals without an m<N> selector decode for every mux value; the switch signal
        // itself decodes always. A frame mux value with no matching group must not drop the
        // message — it still yields switch + common signals.
        var signals = new List<SignalDefinition>(1 + defn.CommonSignals.Length + muxGroup.Length)
        {
            multiplexSignal
        };
        signals.AddRange(defn.CommonSignals);
        signals.AddRange(muxGroup);

        if (signals.Any(s => !SignalBounds.For(s.Signal).FitsIn(data.Length)))
        {
            value = null;
            return false;
        }

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            signals.Select(signal => GetSignalValue(data, signal)).ToArray(),
            switchValue);

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