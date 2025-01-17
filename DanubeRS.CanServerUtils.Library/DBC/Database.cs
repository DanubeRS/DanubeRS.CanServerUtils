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
    private class MessageDefinition(Message message)
    {
        public readonly Signal[] Signals = message.Signals;
        public readonly MessageHeader Header = message.Header;
        public readonly bool IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
    }

    private readonly Dictionary<int, MessageDefinition> _messageDefn = new();

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

    public bool TryParseBinaryMessage(int frameId, byte[] data, [NotNullWhen(true)] out MessageValue? value)
    {
        value = null;
        if (!_messageDefn.TryGetValue(frameId, out var defn))
        {
            return false;
        }

        var bitArray = new BitArray(data);
        if (logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace("Data\n{BitArrayData}", GetBitArrayDataForLog(bitArray));

        return defn.IsMultiplexed switch
        {
            true => TryParseMultiplexedMessage(defn, data, out value),
            false => TryParseRegularMessage(defn, bitArray, out value)
        };
    }

    private bool TryParseRegularMessage(MessageDefinition defn, BitArray data, out MessageValue? value)
    {
        value = null;
        var signalValues = new List<SignalValue>();
        var lastSignal = defn.Signals.OrderByDescending(s => s.StartBit).First();
        // Message size is incompatable
        if (lastSignal.StartBit + lastSignal.Size > data.Length) return false;
        foreach (var signal in defn.Signals.OrderBy(s => s.StartBit))
        {
            var bytes = new byte[8];
            BitsToBytes(data, signal.StartBit, signal.Size, bytes);

            var rawValue = signal.ValueType == ValueType.Unsigned ? (double)BitConverter.ToUInt64(bytes, 0) : (double)BitConverter.ToInt64(bytes, 0);
            
            signalValues.Add(new SignalValue(signal.Name, signal.Offset + rawValue * signal.Factor));
        }

        value = new MessageValue(defn.Header.Id, defn.Header.Name, signalValues.ToArray(), null);
        return true;
    }

    private void BitsToBytes(BitArray bitArray, int offset, int length, byte[] bytes)
    {
        int byteIdx = 0,
         bitIdx = 0;
        
        for (var i = 0; i < length; i++) {
            if (bitArray[offset + i])
                bytes[byteIdx] |= (byte)(1 << bitIdx);

            bitIdx++;
            if (bitIdx != 8) continue;
            bitIdx = 0;
            byteIdx++;
        }
    }

    private bool TryParseMultiplexedMessage(MessageDefinition defn, byte[] data, out MessageValue? value)
    {
        value = default;
        var bitArray = new BitArray(data);
        

        
        var signalValues = new List<SignalValue>();
        var multiplexSignal = defn.Signals.Single(s => s.Multiplex is { IsSwitch: true });

        var switchValue = GetRawValueFromBits(multiplexSignal, bitArray);

        foreach (var signal in defn.Signals.Where(s => s.Multiplex != null && s.Multiplex.SwitchValue == (int)switchValue ))
        {
            var rawValue = GetRawValueFromBits(signal, bitArray);
            var scaledValue = rawValue * signal.Factor + signal.Offset;
            var signalValue = new SignalValue(signal.Name, scaledValue);
            logger.LogDebug("Signal: {Name}, Raw Value: {RawValue}, Scaled Value: {ScaledValue}{Units}", signal.Name,
                rawValue, scaledValue, signal.Unit.Trim('"'));
            signalValues.Add(signalValue);
        }

        value = new MessageValue(defn.Header.Id, defn.Header.Name, signalValues.ToArray(), (uint?)switchValue);

        return true;
    }

    private ulong GetRawValueFromBits(Signal multiplexSignal, BitArray bitArray)
    {
        logger.LogTrace("Decoding {SignalName}", multiplexSignal.Name);
        var switchBits = new BitArray(Enumerable.Range(0, multiplexSignal.Size)
            .Select(i => bitArray.Get(i + multiplexSignal.StartBit)).ToArray());
        var switchBytes = new byte[sizeof(long)];
        switchBits.CopyTo(switchBytes, 0);
        var switchValue = BitConverter.ToUInt64(switchBytes);
        return switchValue;
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