using System.Buffers.Binary;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DanubeRS.CanServerUtils.Lib.DBC.Parser;
using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using Microsoft.Extensions.Logging;
using ValueType = DanubeRS.CanServerUtils.Lib.DBC.Parser.ValueType;

namespace DanubeRS.CanServerUtils.Lib.DBC;

public class Database(ILogger<Database> logger)
{
    public class SignalDefinition(Message message, Signal signal, ParsedDb db)
    {
        public Signal Signal { get; } = signal;
        public string[] Comments { get; } = [];

        public IReadOnlyDictionary<uint, string>? ValueLookup { get; } = db.ValueDescriptions
            .SingleOrDefault(v => v.MessageId == message.Header.Id && v.SignalName == signal.Name)?.Values;
    }
    public class MessageDefinition(Message message, ParsedDb db)
    {
        public readonly SignalDefinition[] Signals = message.Signals.Select(s => new SignalDefinition(message, s, db)).ToArray();
        public readonly MessageHeader Header = message.Header;
        public readonly bool IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
        public readonly string[] Comments = [];
    }

    private readonly Dictionary<uint, MessageDefinition> _messageDefn = new();
    private readonly byte[] _signalValueBytes = new byte[8];

    public void AddFile(TextReader reader)
    {
        var tokenizer = DbcTokenizer.Instance;
        var dbcString = reader.ReadToEnd();
        var tokens = tokenizer.Tokenize(dbcString);
        var parsed = DbcParser.Database(tokens);

        StoreParsedResult(parsed.Value);
    }

    private void StoreParsedResult(ParsedDb parsed)
    {
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
        if (defn.Signals.Length == 0) return true;
        var lastSignal = defn.Signals.OrderByDescending(s => s.Signal.StartBit).First();
        // Message size is incompatable
        if (lastSignal.Signal.StartBit + lastSignal.Signal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            defn.Signals.OrderBy(s => s.Signal.StartBit).Select(signal => GetSignalValue(data, signal)).ToArray(), null);
        return true;
    }

    private SignalValue GetSignalValue(BitArray data, SignalDefinition signal)
    {
        Array.Clear(_signalValueBytes);
        BitsToBytes(data, signal.Signal.StartBit, signal.Signal.Size, _signalValueBytes);
        var rawValue = signal.Signal.ValueType == ValueType.Unsigned
            ? (long)BitConverter.ToUInt64(_signalValueBytes, 0)
            : BitConverter.ToInt64(_signalValueBytes, 0);
        if (signal.Signal.ValueType == ValueType.Signed)
        {
            rawValue = rawValue >> (signal.Signal.Size - 1) == 0 ? rawValue : -1 ^ CreateBitMask(0, signal.Signal.Size) | rawValue;
        }

        var signalValue = new SignalValue(signal.Signal.Name, signal.Signal.Offset + rawValue * signal.Signal.Factor);
        return signalValue;
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
        var bitArray = new BitArray(data);


        var multiplexSignal = defn.Signals.Single(s => s.Signal.Multiplex is { IsSwitch: true });

        var switchBytes = new byte[2];
        BitsToBytes(data, multiplexSignal.Signal.StartBit, multiplexSignal.Signal.Size, switchBytes);
        var switchValue = BitConverter.ToUInt16(switchBytes);
        var filteredSignals = defn.Signals.Where(s => s.Signal.Multiplex != null && s.Signal.Multiplex.SwitchValue == switchValue)
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