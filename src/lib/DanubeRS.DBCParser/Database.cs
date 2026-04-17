using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DanubeRS.CanServerUtils.Lib.Parser;
using DanubeRS.CanServerUtils.Lib.Tokenizer;
using Microsoft.Extensions.Logging;
using ValueType = DanubeRS.CanServerUtils.Lib.Parser.ValueType;

namespace DanubeRS.CanServerUtils.Lib;

public class Database(ILogger<Database> logger)
{
    public class SignalDefinition(Message message, Signal signal, ParsedDb db)
    {
        public Signal Signal { get; } = signal;
        public string[] Comments { get; } = [];

        public IReadOnlyDictionary<uint, string>? ValueLookup { get; } = db.ValueDescriptions
            .SingleOrDefault(v => v.MessageId == message.Header.Id && v.SignalName == signal.Name)?.Values;
    }

    public class MessageDefinition
    {
        private static SignalDefinition CreateSignalDefinition(Message msg, Signal signal, ParsedDb database) => 
            new(msg, signal, database);

        private readonly SignalDefinition[] _signals;
        
        internal MessageDefinition(Message message, ParsedDb db)
        {
            Header = message.Header;
            IsMultiplexed = message.Signals.Any(s => s.Multiplex is { IsSwitch: true });
            
            var signalsList = message.Signals.Select(s => CreateSignalDefinition(message, s, db)).ToList();
            _signals = signalsList.ToArray();
            SignalsByStartBit = signalsList.OrderBy(s => s.Signal.StartBit).ToArray();
        }

        public IReadOnlyList<SignalDefinition> Signals => _signals;
        public SignalDefinition[] SignalsByStartBit { get; }
        public MessageHeader Header { get; }
        public bool IsMultiplexed { get; }
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

    public async Task AddFileAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        var dbcString = await reader.ReadToEndAsync(cancellationToken);
        
        var tokenizer = DbcTokenizer.Instance;
        var tokens = tokenizer.Tokenize(dbcString);
        var parsed = DbcParser.Database(tokens);

        StoreParsedResult(parsed.Value);
    }

    public static async Task<Database> LoadAsync(ILogger<Database> logger, Stream stream, CancellationToken cancellationToken = default)
    {
        var database = new Database(logger);
        await database.AddFileAsync(stream, cancellationToken);
        return database;
    }

    private void StoreParsedResult(ParsedDb parsed)
    {
        foreach (var message in parsed.Messages)
        {
            _messageDefn.TryAdd(message.Header.Id, new MessageDefinition(message, parsed));
        }
    }

    public bool TryParseBinaryMessage(uint frameId, byte[] data, [NotNullWhen(true)] out MessageValue? value,
        [NotNullWhen(true)] out MessageDefinition? defn)
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
        if (defn.Signals.Count == 0) return true;
        var lastSignal = defn.Signals.OrderByDescending(s => s.Signal.StartBit).First();
        // Message size is incompatable
        if (lastSignal.Signal.StartBit + lastSignal.Signal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            defn.SignalsByStartBit.Select(signal => GetSignalValue(data, signal)).ToArray(),
            null);
        return true;
    }

    private SignalValue GetSignalValue(BitArray data, SignalDefinition signal)
    {
        Array.Clear(_signalValueBytes);

        switch (signal.Signal.Order)
        {
            case Endianness.Little:
                ExtractBitsLittleEndian(data, signal.Signal.StartBit, signal.Signal.Size, _signalValueBytes);
                break;
            case Endianness.Big:
                ExtractBitsBigEndian(data, signal.Signal.StartBit, signal.Signal.Size, _signalValueBytes);
                break;
        }

        long rawValue;
        if (signal.Signal.ValueType == ValueType.Unsigned)
            rawValue = (long)BitConverter.ToUInt64(_signalValueBytes, 0);
        else
        {
            var valueAsInt64 = BitConverter.ToInt64(_signalValueBytes, 0);
            var shiftAmount = 64 - signal.Signal.Size;
            rawValue = (valueAsInt64 << shiftAmount) >> shiftAmount;
        }

        return new SignalValue(signal.Signal.Name, signal.Signal.Offset + rawValue * signal.Signal.Factor);
    }

    private static void ExtractBitsLittleEndian(BitArray bitArray, int startBit, int signalSize, byte[] outputBytes)
    {
        ulong value = 0;

        for (int i = 0; i < signalSize && (startBit + i) / 8 < bitArray.Count; i++)
        {
            if (bitArray[startBit + i])
                value |= (ulong)(1UL << i); // First extracted bit is LSB for little-endian
        }

        for (int i = 0; i < Math.Min((signalSize + 7) / 8, outputBytes.Length); i++)
            outputBytes[i] = (byte)((value >> (i * 8)) & 0xFF);
    }

     private static void ExtractBitsBigEndian(BitArray bitArray, int startBit, int signalSize, byte[] outputBytes)
    {
        ulong value = 0;

        for (int i = 0; i < signalSize && (startBit + i) / 8 < bitArray.Count; i++)
        {
            int frameBitPos = startBit + i;
            int sourceByteIdx = frameBitPos / 8;
            int sourceBitInByte = frameBitPos % 8;

            // For big-endian, first extracted bit is MSB of signal value
            if (bitArray[frameBitPos])
                value |= (ulong)(1UL << (signalSize - 1 - i));
        }

        for (int i = 0; i < Math.Min((signalSize + 7) / 8, outputBytes.Length); i++)
            outputBytes[i] = (byte)((value >> (i * 8)) & 0xFF);
    }

    private bool TryParseMultiplexedMessage(MessageDefinition defn, BitArray data, out MessageValue? value)
    {
        value = null;

        var multiplexSignal = defn.Signals.FirstOrDefault(s => s.Signal.Multiplex is { IsSwitch: true });
        if (multiplexSignal == null)
            return false;

        switch (multiplexSignal.Signal.Order)
        {
            case Endianness.Little:
                ExtractBitsLittleEndian(data, multiplexSignal.Signal.StartBit, multiplexSignal.Signal.Size, _signalValueBytes);
                break;
            case Endianness.Big:
                ExtractBitsBigEndian(data, multiplexSignal.Signal.StartBit, multiplexSignal.Signal.Size, _signalValueBytes);
                break;
        }
        
        // Extract mux value based on signal size (can be 8-64 bits)
        ulong rawSwitchValue;
        if (multiplexSignal.Signal.Size <= 8)
            rawSwitchValue = _signalValueBytes[0];
        else if (multiplexSignal.Signal.Size <= 16)
            rawSwitchValue = BitConverter.ToUInt16(_signalValueBytes);
        else if (multiplexSignal.Signal.Size <= 32)
            rawSwitchValue = BitConverter.ToUInt32(_signalValueBytes);
        else
            rawSwitchValue = BitConverter.ToUInt64(_signalValueBytes);

        var switchValue = (uint)rawSwitchValue;
        
        // Include: mux switch signal, always-present signals (no multiplex), and matching muxed signals  
        var filteredSignals = defn.Signals
            .Where(s => s.Signal.Multiplex?.IsSwitch == true 
                      || s.Signal.Multiplex == null 
                      || s.Signal.Multiplex.SwitchValue == switchValue)
            .ToArray();

        if (filteredSignals.Length == 0)
            return false;

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

public record MessageValue(uint MessageId, string MessageName, SignalValue[] Signals, uint? MultiplexValue);