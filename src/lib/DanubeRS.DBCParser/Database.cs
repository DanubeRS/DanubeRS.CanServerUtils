using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DanubeRS.CanServerUtils.Lib.Parser;
using DanubeRS.CanServerUtils.Lib.Tokenizer;
using DanubeRS.DBCParser;
using Microsoft.Extensions.Logging;


namespace DanubeRS.CanServerUtils.Lib;

public class Database(ILogger<Database> logger)
{
    public class SignalDefinition(Message message, Signal signal, ParsedDb db)
    {
        public Signal Signal { get; } = signal;
        public string[] Comments { get; } = [];

        public IReadOnlyDictionary<uint, string>? ValueLookup { get; } = db.ValueDescriptions
            .FirstOrDefault(v => v.MessageId == message.Header.Id && v.SignalName == signal.Name)?.Values;
    }

    public class MessageDefinition
    {
        private static SignalDefinition CreateSignalDefinition(Message msg, Signal signal, ParsedDb database) => 
            new(msg, signal, database);

        private readonly SignalDefinition[] _signals;
        
        public MessageDefinition(Message message, ParsedDb db)
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

    public void AddFile(TextReader reader)
    {
        var tokenizer = DbcTokenizer.Instance;
        var dbcString = reader.ReadToEnd();
        var tokens = tokenizer.Tokenize(dbcString);
        var parsed = DbcParser.Database(tokens);

        if (!parsed.HasValue)
            throw new DbcParseException(parsed.ErrorMessage != null
                ? [parsed.ErrorMessage]
                : parsed.Expectations != null
                    ? parsed.Expectations
                    : ["Unknown parse error"]);

        StoreParsedResult(parsed.Value);
    }

    public async Task AddFileAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        var dbcString = await reader.ReadToEndAsync(cancellationToken);
        
        var tokenizer = DbcTokenizer.Instance;
        var tokens = tokenizer.Tokenize(dbcString);
        var parsed = DbcParser.Database(tokens);

        if (!parsed.HasValue)
            throw new DbcParseException(parsed.ErrorMessage != null
                ? [parsed.ErrorMessage]
                : parsed.Expectations != null
                    ? parsed.Expectations
                    : ["Unknown parse error"]);

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

        if (logger.IsEnabled(LogLevel.Trace))
        {
            var bitArray = new BitArray(data);
            logger.LogTrace("Frame 0x{FrameId:X3} ({MessageName}) Data\n{BitArrayData}",
                defn.Header.Id, defn.Header.Name, GetBitArrayDataForLog(bitArray));
        }

        return defn.IsMultiplexed switch
        {
            true => Decoder.TryParseMultiplexedMessage(defn, data, out value),
            false => Decoder.TryParseRegularMessage(defn, data, out value)
        };
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