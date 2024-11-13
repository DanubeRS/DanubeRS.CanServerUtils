using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using DanubeRS.CanServerUtils.Lib.DBC.Parser;
using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using ValueType = DanubeRS.CanServerUtils.Lib.DBC.Parser.ValueType;

namespace DanubeRS.CanServerUtils.Lib.DBC;

public class Database
{
    private class MessageDefinition(Message message)
    {
        public readonly Signal[] Signals = message.Signals;
        public readonly MessageHeader Header = message.Header;
        public readonly bool IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
    }

    private Dictionary<int, MessageDefinition> _messageDefn = new Dictionary<int, MessageDefinition>();

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

        if (defn.IsMultiplexed)
        {
            return TryParseMultiplexedMessage(defn, data, out value);
        }

        return TryParseRegularMessage(defn, data, out value);
    }

    private bool TryParseRegularMessage(MessageDefinition defn, byte[] data, out MessageValue? value)
    {
        var signalValues = new List<SignalValue>();
        foreach (var signal in defn.Signals.OrderBy(s => s.StartBit))
        {
            signalValues.Add(new SignalValue(signal.Name));
        }

        value = new MessageValue(defn.Header.Id, defn.Header.Name, signalValues.ToArray(), null);
        return true;
    }

    private bool TryParseMultiplexedMessage(MessageDefinition defn, byte[] data, out MessageValue? value)
    {
        value = default;
        var signalValues = new List<SignalValue>();
        var multiplexSignal = defn.Signals.Single(s => s.Multiplex is { IsSwitch: true });

        // TODO need to handle larger (if possible?) and cross-byte multiplex values
        return false;
        int multiplexByte = multiplexSignal.StartBit / sizeof(byte);
        var multiplexBit = multiplexSignal.StartBit % sizeof(byte);
        int multiplexValue = data[multiplexByte] >> (sizeof(byte) - multiplexBit - multiplexSignal.Size);

        if (multiplexSignal.Order == Endianness.Little)


            foreach (var signal in defn.Signals.Where(s =>
                         s.Multiplex != null && s.Multiplex.SwitchValue == multiplexValue))
            {
                signalValues.Add(new SignalValue(signal.Name));
            }

        value = new MessageValue(defn.Header.Id, defn.Header.Name, signalValues.ToArray(), null);
        return true;
    }
}

public record SignalValue(string SignalName);

public record MessageValue(int MessageId, string MessageName, SignalValue[] Signals, int? MultiplexValue);