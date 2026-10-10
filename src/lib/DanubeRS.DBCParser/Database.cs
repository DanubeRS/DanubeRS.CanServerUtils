using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DanubeRS.DBCParser.Parser;
using DanubeRS.DBCParser.Tokenizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ValueType = DanubeRS.DBCParser.Parser.ValueType;

namespace DanubeRS.DBCParser;

public class Database
{
    private readonly ILogger<Database> _logger;

    public Database(ILogger<Database>? logger = null)
    {
        // Pure-library convenience: no explicit logger is fine (dup-ID warnings vanish).
        _logger = logger ?? NullLogger<Database>.Instance;
    }

    /// <summary>CAN identifiers are unsigned 11/29-bit values; file IDs carry the extended flag, joins mask it.</summary>
    internal static uint MaskedId(uint id) => id & 0x1FFFFFFFu;

    public class SignalDefinition(Message message, Signal signal, ParsedDb db)
    {
        public Signal Signal { get; } = signal;

        /// <summary>Endianness-aware bit span of this signal.</summary>
        public SignalBounds Bounds { get; } = SignalBounds.For(signal);

        public string[] Comments { get; } = db.Comments
            .OfType<SignalComment>()
            .Where(c => MaskedId(c.MessageId) == MaskedId(message.Header.Id) && c.SignalName == signal.Name)
            .Select(c => c.Comment)
            .ToArray();

        public IReadOnlyDictionary<long, string>? ValueLookup { get; } = db.ValueDescriptions
            .FirstOrDefault(v => MaskedId(v.MessageId ?? 0) == MaskedId(message.Header.Id) && v.SignalName == signal.Name)?.Values;
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
                maxBit = Math.Max(maxBit, bitPos);
                minBit = Math.Min(minBit, bitPos);
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
                .Where(c => MaskedId(c.MessageId) == MaskedId(message.Header.Id))
                .Select(c => c.Comment)
                .ToArray();

            CommonSignals = Signals.Where(sd => sd.Signal.Multiplex is null).ToArray();
            MultiplexSignal = Signals.FirstOrDefault(sd => sd.Signal.Multiplex is { IsSwitch: true });
            MuxGroups = Signals.Where(sd => sd.Signal.Multiplex is { IsSwitch: false })
                .GroupBy(sd => (uint)sd.Signal.Multiplex!.SwitchValue!.Value)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }

        /// <summary>Signals ordered by start bit (master-side shape kept for Decoder.cs iteration).</summary>
        public SignalDefinition[] SignalsByStartBit => Signals.OrderBy(s => s.Signal.StartBit).ToArray();
    }

    private readonly Dictionary<uint, MessageDefinition> _messageDefn = new();

    /// <summary>
    /// The stored key is the 29-bit CAN ID (file IDs masked with 0x1FFFFFFF: DBC files
    /// conventionally set the 0x80000000 extended-frame flag, but a frame coming off the bus
    /// carries the bare 29-bit ID). The file's exact ID is preserved on MessageHeader.
    /// </summary>
    private static uint LookupKey(uint id) => id & 0x1FFFFFFFu;

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
    /// Keywords of recognized-but-unmodeled sections that were skipped during parsing
    /// (e.g. VAL_TABLE_, SG_MUL_VAL_, SIG_VALTYPE_, BO_TX_BU_, EV_, ENVVAR_DATA_).
    /// </summary>
    public ImmutableArray<string> SkippedStatements { get; private set; } = [];

    /// <summary>
    /// Version string from the file.
    /// </summary>
    public string? Version { get; private set; }

    public void AddFile(TextReader reader)
    {
        var dbcString = reader.ReadToEnd();
        var tokens = DbcTokenizer.Instance.TryTokenize(dbcString);
        if (!tokens.HasValue)
            throw new DbcParseException(
                [$"Failed to tokenize DBC file at position {tokens.ErrorPosition.Absolute} (line {tokens.ErrorPosition.Line}, column {tokens.ErrorPosition.Column}): {tokens.FormatErrorMessageFragment()}. Context: ...{ContextSnippet(dbcString, tokens.ErrorPosition.Absolute)}..."]);

        var parsed = DbcParser.Database(tokens.Value);
        if (!parsed.HasValue)
            throw new DbcParseException(
                [$"Failed to parse DBC file at position {parsed.ErrorPosition.Absolute} (line {parsed.ErrorPosition.Line}, column {parsed.ErrorPosition.Column}): expected {parsed.FormatErrorMessageFragment()}. Context: ...{ContextSnippet(dbcString, parsed.ErrorPosition.Absolute)}..."]);

        StoreParsedResult(parsed.Value);
    }

    public async Task AddFileAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        var dbcString = await reader.ReadToEndAsync(cancellationToken);
        
        var tokens = DbcTokenizer.Instance.TryTokenize(dbcString);
        if (!tokens.HasValue)
            throw new DbcParseException(
                [$"Failed to tokenize DBC file at position {tokens.ErrorPosition.Absolute} (line {tokens.ErrorPosition.Line}, column {tokens.ErrorPosition.Column}): {tokens.FormatErrorMessageFragment()}. Context: ...{ContextSnippet(dbcString, tokens.ErrorPosition.Absolute)}..."]);

        var parsed = DbcParser.Database(tokens.Value);
        if (!parsed.HasValue)
            throw new DbcParseException(
                [$"Failed to parse DBC file at position {parsed.ErrorPosition.Absolute} (line {parsed.ErrorPosition.Line}, column {parsed.ErrorPosition.Column}): expected {parsed.FormatErrorMessageFragment()}. Context: ...{ContextSnippet(dbcString, parsed.ErrorPosition.Absolute)}..."]);

        StoreParsedResult(parsed.Value);
    }

    private static string ContextSnippet(string dbcString, int position) =>
        dbcString[Math.Max(0, position - 40)..Math.Min(position + 60, dbcString.Length)]
            .Replace("\r", "").Replace("\n", "⏎");

    public static async Task<Database> LoadAsync(ILogger<Database> logger, Stream stream, CancellationToken cancellationToken = default)
    {
        var database = new Database(logger);
        await database.AddFileAsync(stream, cancellationToken);
        return database;
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
        SkippedStatements = SkippedStatements.AddRange(parsed.SkippedStatements);
        Version ??= parsed.Version.VersionString;

        foreach (var message in parsed.Messages)
        {
            var id = LookupKey(message.Header.Id);
            if (!_messageDefn.TryAdd(id, new MessageDefinition(message, parsed)))
            {
                // First definition wins; make the dropped duplicate visible.
                _logger.LogWarning(
                    "Duplicate definition for CAN frame {FrameId} ({MessageName}) ignored; keeping the first definition",
                    message.Header.Id, message.Header.Name);
            }
        }
    }

    public bool TryParseBinaryMessage(uint frameId, byte[] data, [NotNullWhen(true)] out MessageValue? value,
        [NotNullWhen(true)] out MessageDefinition? defn)
    {
        value = null;
        if (!_messageDefn.TryGetValue(LookupKey(frameId), out defn))
        {
            return false;
        }

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            var bitArray = new BitArray(data);
            _logger.LogTrace("Frame 0x{FrameId:X3} ({MessageName}) Data\n{BitArrayData}",
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
