using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DanubeRS.CanServerUtils.Lib.DBC.Parser;
using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using Microsoft.Extensions.Logging;
using ValueType = DanubeRS.CanServerUtils.Lib.DBC.Parser.ValueType;

namespace DanubeRS.CanServerUtils.Lib.DBC;

public class Database(ILogger<Database> logger)
{
    public class MessageDefinition(Message message)
    {
        public readonly Signal[] Signals = message.Signals;
        public readonly MessageHeader Header = message.Header;
        public readonly bool IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
    }

    private readonly Dictionary<int, MessageDefinition> _messageDefn = new();
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
            _messageDefn.TryAdd(message.Header.Id, new MessageDefinition(message));
        }
    }

    public bool TryParseBinaryMessage(int frameId, byte[] data, [NotNullWhen(true)] out MessageValue? value, [NotNullWhen(true)] out MessageDefinition? defn)
    {
        value = null;
        if (!_messageDefn.TryGetValue(frameId, out defn))
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
        var lastSignal = defn.Signals.OrderByDescending(s => s.StartBit).First();
        // Message size is incompatable
        if (lastSignal.StartBit + lastSignal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            defn.Signals.OrderBy(s => s.StartBit).Select(signal => GetSignalValue(data, signal)).ToArray(), null);
        return true;
    }

    private SignalValue GetSignalValue(BitArray data, Signal signal)
    {
        Array.Clear(_signalValueBytes);
        BitsToBytes(data, signal.StartBit, signal.Size, _signalValueBytes);
        var rawValue = signal.ValueType == ValueType.Unsigned
            ? BitConverter.ToUInt64(_signalValueBytes, 0)
            : (double)BitConverter.ToInt64(_signalValueBytes, 0);
        var signalValue = new SignalValue(signal.Name, signal.Offset + rawValue * signal.Factor);
        return signalValue;
    }

    private void BitsToBytes(BitArray bitArray, int offset, int length, byte[] bytes)
    {
        int byteIdx = 0,
            bitIdx = 0;

        for (var i = 0; i < length; i++)
        {
            if (bitArray[offset + i])
                bytes[byteIdx] |= (byte)(1 << bitIdx);

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


        var multiplexSignal = defn.Signals.Single(s => s.Multiplex is { IsSwitch: true });

        var switchBytes = new byte[2];
        BitsToBytes(data, multiplexSignal.StartBit, multiplexSignal.Size, switchBytes);
        var switchValue = BitConverter.ToUInt16(switchBytes);
        var filteredSignals = defn.Signals.Where(s => s.Multiplex != null && s.Multiplex.SwitchValue == switchValue)
            .ToArray();

        var lastSignal = filteredSignals.OrderByDescending(s => s.StartBit).FirstOrDefault();
        if (lastSignal == null) return false;
        // Message size is incompatable
        if (lastSignal.StartBit + lastSignal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            filteredSignals.Select(signal => GetSignalValue(data, signal)).ToArray(), (uint?)switchValue);

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