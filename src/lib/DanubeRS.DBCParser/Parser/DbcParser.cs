using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using DanubeRS.DBCParser.Tokenizer;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace DanubeRS.DBCParser.Parser;

public static class DbcParser
{
    private static TokenListParser<DbcTokens, Version> Version { get; } =
        from versionKeyword in NamedIdentifier("VERSION")
        from version in QuotedString
        from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select new Version(version);

    private static TokenListParser<DbcTokens, NewSymbols> NewSymbols { get; } =
        from nsKeyword in Token.EqualToValue(DbcTokens.Identifier, "NS_")
        from _ in Token.EqualTo(DbcTokens.Colon)
        from values in Token.EqualTo(DbcTokens.Identifier)
            .Or(Token.EqualTo(DbcTokens.Identifier))
            .Many()
        from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select new NewSymbols(values.Select(v => v.ToStringValue()).ToArray());

    private static TokenListParser<DbcTokens, Unit> BitTiming { get; } =
        from bsKeyword in Token.EqualToValue(DbcTokens.Identifier, "BS_")
        from colon in Token.EqualTo(DbcTokens.Colon)
        from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select Unit.Value;

    private static TokenListParser<DbcTokens, Nodes> Nodes { get; } =
        from buKeyword in Token.EqualToValue(DbcTokens.Identifier, "BU_")
        from colon in Token.EqualTo(DbcTokens.Colon)
        from nodes in Token.EqualTo(DbcTokens.Identifier)
            .Many()
        from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select new Nodes(nodes.Select(t => t.ToStringValue()).ToArray());

    private static TokenListParser<DbcTokens, MessageHeader> MessageHeader { get; } =
        from boKeyword in Token.EqualToValue(DbcTokens.Identifier, "BO_")
        from messageId in MessageId
        from messageName in Token.EqualTo(DbcTokens.Identifier)
        from colon in Token.EqualTo(DbcTokens.Colon)
        from messageSize in Integer
        from nodeName in Token.EqualTo(DbcTokens.Identifier)
        select new MessageHeader(messageId, messageName.ToStringValue(),
            messageSize, nodeName.ToStringValue());

    /// <summary>CAN identifiers are unsigned 11/29-bit values; parse without Int32 overflow.</summary>
    private static TokenListParser<DbcTokens, uint> MessageId { get; } =
        Token.EqualTo(DbcTokens.Integer)
            .Or(Token.EqualTo(DbcTokens.Double).Where(t =>
                uint.TryParse(t.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
            .Where(t => uint.TryParse(t.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            .Select(t => uint.Parse(t.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture));

    // If has value, then it is a multiplexed signal, if -1, it is the multiplexer
    private static Regex MultiplexedRegex { get; } = new Regex(@"m([\d]+)");

    private static TokenListParser<DbcTokens, string> Identifier { get; } =
        Token.EqualTo(DbcTokens.Identifier).Select(t => t.ToStringValue());

    private static TokenListParser<DbcTokens, string> NamedIdentifier(string name) =>
        Token.EqualToValue(DbcTokens.Identifier, name).Select(t => t.ToStringValue());

    private static TokenListParser<DbcTokens, Multiplex> Multiplexed { get; } =
        Identifier.Where(t => MultiplexedRegex.IsMatch(t))
            .Select(t => int.Parse(MultiplexedRegex.Match(t).Groups[1].Value)).Select(m => new Multiplex(false, m));

    private static TokenListParser<DbcTokens, Multiplex> Multiplexer { get; } =
        NamedIdentifier("M").Value(new Multiplex(true, null));

    private static TokenListParser<DbcTokens, Multiplex> Multiplex { get; } =
        Multiplexed.Or(Multiplexer);

    private static TokenListParser<DbcTokens, int> Integer { get; } =
        Token.EqualTo(DbcTokens.Integer).Or(Token.EqualTo(DbcTokens.Double))
            .Where(t => int.TryParse(t.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            .Select(t => int.Parse(t.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture));

    private static TokenListParser<DbcTokens, double> Scientific { get; } =
        Token.EqualTo(DbcTokens.Sci).Select(t => double.Parse(t.ToStringValue(), NumberStyles.Float, CultureInfo.InvariantCulture));

    private static TokenListParser<DbcTokens, double> Double { get; } = Token.EqualTo(DbcTokens.Integer)
        .Or(Token.EqualTo(DbcTokens.Double))
        .Select(t => double.Parse(t.ToStringValue(), NumberStyles.Float, CultureInfo.InvariantCulture));

    private static TokenListParser<DbcTokens, bool> Boolean { get; } = Integer.Select(Convert.ToBoolean);

    private static TokenListParser<DbcTokens, ValueType> ValueType = Token.EqualTo(DbcTokens.Sign)
        .Select(s => s.ToStringValue() == "+" ? Parser.ValueType.Unsigned : Parser.ValueType.Signed);

    private static TokenListParser<DbcTokens, Endianness> Endianness { get; } =
        Boolean.Select(b => b ? Parser.Endianness.Little : Parser.Endianness.Big);

    private static TokenListParser<DbcTokens, string> QuotedString { get; } =
        Token.EqualTo(DbcTokens.String)
            .Select(s =>
            {
                // Token span includes the surrounding quotes; strip them and unescape \" escapes.
                var text = s.ToStringValue();
                if (text.Length >= 2 && text[0] == '"' && text[^1] == '"')
                {
                    var content = text[1..^1];
                    return content.Replace("\\\"", "\"").Replace("\\\\", "\\");
                }
                return text;
            });

    private static TokenListParser<DbcTokens, string> ReceiverList { get; } =
        Identifier.ManyDelimitedBy(Token.EqualTo(DbcTokens.Comma))
            .Select(receivers => string.Join(", ", receivers));

    private static TokenListParser<DbcTokens, Signal> Signal { get; } =
        from sgKeyword in Token.EqualToValue(DbcTokens.Identifier, "SG_")
        from signalName in Identifier
        from multiplexIdentifier in Multiplex.OptionalOrDefault()
        from colon in Token.EqualTo(DbcTokens.Colon)
        from startBit in Integer
        from pipe in Token.EqualTo(DbcTokens.Pipe)
        from signalSize in Integer
        from orderDelim in Token.EqualTo(DbcTokens.AtSymbol)
        from order in Endianness
        from valueType in ValueType
        from lParen in Token.EqualTo(DbcTokens.LParen)
        from factor in Double.Or(Scientific)
        from factorOffsetComma in Token.EqualTo(DbcTokens.Comma)
        from offset in Double.Or(Scientific)
        from rParen in Token.EqualTo(DbcTokens.RParen)
        from lSquParen in Token.EqualTo(DbcTokens.LSqParen)
        from min in Double.Or(Scientific)
        from minMaxPipe in Token.EqualTo(DbcTokens.Pipe)
        from max in Double.Or(Scientific)
        from rSquParen in Token.EqualTo(DbcTokens.RSqParen)
        from unit in QuotedString
        from receivers in ReceiverList
        select new Signal(signalName, multiplexIdentifier, startBit, signalSize, order, valueType,
            factor, offset, min, max, unit, receivers);

    private static TokenListParser<DbcTokens, Message> Message { get; } =
        from header in MessageHeader
        from signals in Signal.Many()
        from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select new Message(header,
            signals.OrderBy(s => s.Multiplex?.SwitchValue ?? -1).ThenBy(s => s.StartBit).ToArray());

    private static TokenListParser<DbcTokens, CommentBase> SimpleComment { get; } =
        from comment in QuotedString
        select (CommentBase)new SimpleComment(comment);

    private static TokenListParser<DbcTokens, CommentBase> NodeComment { get; } =
        from keyword in NamedIdentifier("BU_")
        from nodeName in Identifier
        from comment in QuotedString
        select (CommentBase)new NodeComment(nodeName, comment);

    private static TokenListParser<DbcTokens, CommentBase> MessageComment { get; } =
        from keyword in NamedIdentifier("BO_")
        from messageId in MessageId
        from comment in QuotedString
        select (CommentBase)new MessageComment(messageId, comment);

    private static TokenListParser<DbcTokens, CommentBase> SignalComment { get; } =
        from keyword in NamedIdentifier("SG_")
        from messageId in MessageId
        from signalName in Identifier
        from comment in QuotedString
        select (CommentBase)new SignalComment(messageId, signalName, comment);

    private static TokenListParser<DbcTokens, CommentBase> EnvironmentComment { get; } =
        from keyword in NamedIdentifier("EV_")
        from varName in Identifier
        from comment in QuotedString
        select (CommentBase)new EnvironmentComment(varName, comment);


    private static TokenListParser<DbcTokens, CommentBase> Comment { get; } =
        from cmKeyword in NamedIdentifier("CM_")
        from comment in
            SimpleComment
                .Or(NodeComment)
                .Or(MessageComment)
                .Or(SignalComment)
                .Or(EnvironmentComment)
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select comment;

    private static TokenListParser<DbcTokens, string> AttributeName { get; } =
        from attr in QuotedString
        select attr;

    private static TokenListParser<DbcTokens, AttributeValueType> IntAttributeValueType { get; } =
        from keyword in NamedIdentifier("INT")
        from min in Integer
        from max in Integer
        where min <= max
        select (AttributeValueType)new IntAttributeValueType(min, max);

    private static TokenListParser<DbcTokens, AttributeValueType> HexAttributeValueType { get; } =
        from keyword in NamedIdentifier("HEX")
        from min in Integer
        from max in Integer
        where min <= max
        select (AttributeValueType)new HexAttributeValueType(min, max);

    private static TokenListParser<DbcTokens, AttributeValueType> FloatAttributeValueType { get; } =
        from keyword in NamedIdentifier("FLOAT")
        from min in Double
        from max in Double
        where min <= max
        select (AttributeValueType)new FloatAttributeValueType(min, max);

    private static TokenListParser<DbcTokens, AttributeValueType> StringAttributeValueType { get; } =
        NamedIdentifier("STRING").Value((AttributeValueType)new StringAttributeValueType());

    private static TokenListParser<DbcTokens, AttributeValueType> EnumAttributeValueType { get; } =
        from keyword in NamedIdentifier("ENUM")
        from values in QuotedString.ManyDelimitedBy(Token.EqualTo(DbcTokens.Comma))
        select (AttributeValueType)new EnumAttributeValueType(values);

    private static TokenListParser<DbcTokens, AttributeValueType> AttributeValueType { get; } =
        IntAttributeValueType
            .Or(HexAttributeValueType)
            .Or(FloatAttributeValueType)
            .Or(StringAttributeValueType)
            .Or(EnumAttributeValueType);

    private static TokenListParser<DbcTokens, AttributeDefinition> AttributeDefinition { get; } =
        from keyword in NamedIdentifier("BA_DEF_")
        from objectType in
            NamedIdentifier("BU_")
                .Or(NamedIdentifier("BO_"))
                .Or(NamedIdentifier("SG_"))
                .Or(NamedIdentifier("EV_"))
                .OptionalOrDefault()
        from attributeName in AttributeName
        from attributeValueType in AttributeValueType
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select new AttributeDefinition(objectType, attributeName, attributeValueType);

    private static TokenListParser<DbcTokens, AttributeDefaultValue> IntegerAttributeDefaultValue { get; } =
        // Numeric tokens arrive as Double even for plain integers (Numerics.DecimalDouble is
        // matched before Numerics.Integer), so accept either token kind whose text is integral.
        // Integer tokens must also carry integral text ("1.0" style text stays Double).
        Token.EqualTo(DbcTokens.Integer)
            .Or(Token.EqualTo(DbcTokens.Double).Where(i =>
                int.TryParse(i.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
            .Where(i => int.TryParse(i.ToStringValue(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            .Select(i =>
                (AttributeDefaultValue)new IntegerAttributeDefaultValue(int.Parse(i.ToStringValue(), CultureInfo.InvariantCulture)));

    private static TokenListParser<DbcTokens, AttributeDefaultValue> DoubleAttributeDefaultValue { get; } =
        Token.EqualTo(DbcTokens.Double).Select(i =>
            (AttributeDefaultValue)new DoubleAttributeDefaultValue(double.Parse(i.ToStringValue(), CultureInfo.InvariantCulture)));

    private static TokenListParser<DbcTokens, AttributeDefaultValue> StringAttributeDefaultValue { get; } =
        QuotedString.Select(i => (AttributeDefaultValue)new StringAttributeDefaultValue(i));

    private static TokenListParser<DbcTokens, AttributeDefaultValue> AttributeDefaultValue { get; } =
        IntegerAttributeDefaultValue.Or(DoubleAttributeDefaultValue).Or(StringAttributeDefaultValue);

    private static TokenListParser<DbcTokens, AttributeDefault> AttributeDefaults { get; } =
        from keyword in NamedIdentifier("BA_DEF_DEF_")
        from attributeName in QuotedString
        from attributeDefaultValue in AttributeDefaultValue
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select new AttributeDefault(attributeName, attributeDefaultValue);

    private static TokenListParser<DbcTokens, AttributeObjectValue> GlobalObjectValue { get; } =
        from value in AttributeDefaultValue
        select (AttributeObjectValue)new GlobalObjectValue(value);

    private static TokenListParser<DbcTokens, AttributeObjectValue> NodeObjectValue { get; } =
        from keyword in NamedIdentifier("BU_")
        from name in Identifier
        from value in AttributeDefaultValue
        select (AttributeObjectValue)new NodeObjectValue(name, value);

    private static TokenListParser<DbcTokens, AttributeObjectValue> MessageObjectValue { get; } =
        from keyword in NamedIdentifier("BO_")
        from messageId in MessageId
        from value in AttributeDefaultValue
        select (AttributeObjectValue)new MessageObjectValue(messageId, value);

    private static TokenListParser<DbcTokens, AttributeObjectValue> SignalObjectValue { get; } =
        from keyword in NamedIdentifier("SG_")
        from messageId in MessageId
        from signalName in Identifier
        from value in AttributeDefaultValue
        select (AttributeObjectValue)new SignalObjectValue(messageId, signalName, value);

    private static TokenListParser<DbcTokens, AttributeObjectValue> EnvironmentObjectValue { get; } =
        from keyword in NamedIdentifier("EV_")
        from name in Identifier
        from value in AttributeDefaultValue
        select (AttributeObjectValue)new EnvironmentObjectValue(name, value);

    private static TokenListParser<DbcTokens, AttributeObjectValue> AttributeObjectValue { get; } =
        GlobalObjectValue.Or(NodeObjectValue).Or(MessageObjectValue).Or(SignalObjectValue).Or(EnvironmentObjectValue);

    private static TokenListParser<DbcTokens, AttributeValue> AttributeValue { get; } =
        from keyword in NamedIdentifier("BA_")
        from attributeName in QuotedString
        from attributeValue in AttributeObjectValue
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select new AttributeValue(attributeName, attributeValue);

    private static TokenListParser<DbcTokens, ValueDescription> SignalValueDescription { get; } =
        from keyword in NamedIdentifier("VAL_")
        from messageId in MessageId
        from signalName in Identifier
        from valueDescriptionValues in Integer
            .Then((i => QuotedString.Select(s => new KeyValuePair<long, string>(i, s)))).Many()
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select new ValueDescription(messageId, signalName,
            ParseValueDescriptionValues(valueDescriptionValues));

    private static TokenListParser<DbcTokens, ValueDescription> EnvValueDescription { get; } =
        from keyword in NamedIdentifier("VAL_")
        from envVarName in Identifier
        from valueDescriptionValues in Integer
            .Then((i => QuotedString.Select(s => new KeyValuePair<long, string>(i, s)))).Many()
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select new ValueDescription(null, envVarName,
            ParseValueDescriptionValues(valueDescriptionValues));

    private static TokenListParser<DbcTokens, ValueDescription> ValueDescription { get; } =
        SignalValueDescription.Try().Or(EnvValueDescription);

    private static IImmutableDictionary<long, string> ParseValueDescriptionValues(KeyValuePair<long, string>[] values)
    {
        // Last entry wins for duplicate keys (spec is silent; Vector tools keep the last).
        var builder = ImmutableDictionary.CreateBuilder<long, string>();
        foreach (var (key, value) in values)
            builder[key] = value;
        return builder.ToImmutable();
    }

    /// <summary>
    /// Catch-all for well-formed but unmodeled sections (VAL_TABLE_, SIG_VALTYPE_, SG_MUL_VAL_,
    /// BO_TX_BU_, EV_, ENVVAR_DATA_, BA_*_REL_, ...): a keyword identifier plus all tokens up to
    /// (and including) a semicolon, or a definition-end. Skipped content is recorded in
    /// ParsedDb.SkippedStatements — nothing silently lost. Malformed KNOWN sections must NOT be
    /// swallowed by this branch, so reserved keywords are rejected here.
    /// </summary>
    private static readonly string[] ReservedKeywords =
    {
        "VERSION", "NS_", "BS_", "BU_", "BO_", "SG_", "CM_", "BA_DEF_", "BA_DEF_DEF_", "BA_", "VAL_"
    };

    private static readonly TokenListParser<DbcTokens, UnknownStatement> UnknownStatement =
        input =>
        {
            // Only an Identifier can head an unknown section; DefinitionEnd (blank line) is a
            // separator handled by the statement loop, never a statement.
            var peek = input.ConsumeToken();
            if (!peek.HasValue || peek.Value.Kind != DbcTokens.Identifier)
                return TokenListParserResult.Empty<DbcTokens, UnknownStatement>(input);

            var name = peek.Value.ToStringValue();
            if (Array.IndexOf(ReservedKeywords, name) >= 0)
            {
                // A known section keyword: let the typed branches (or AtEnd) report the error
                // instead of silently swallowing it here.
                return TokenListParserResult.Empty<DbcTokens, UnknownStatement>(input);
            }

            var rest = peek.Remainder;
            var count = 0;
            while (true)
            {
                var token = rest.ConsumeToken();
                if (!token.HasValue)
                {
                    return TokenListParserResult.Empty<DbcTokens, UnknownStatement>(rest, "`;` or section terminator");
                }
                rest = token.Remainder;
                count++;
                if (token.Value.Kind == DbcTokens.Semicolon)
                    break;
                if (token.Value.Kind == DbcTokens.DefinitionEnd)
                    break; // consumed the section terminator
            }

            return TokenListParserResult.Value(new UnknownStatement(name, count - 1), input, rest);
        };

    public static TokenListParser<DbcTokens, ParsedDb> Database { get; } = BuildDatabaseParser();

    // NOTE: static field initializers run in textual order; `Database` must come last,
    // after every parser it references. `Statement` is a local func-built parser.

    private static TokenListParser<DbcTokens, ToplevelStatement> BuildStatement() =>
        Version.Try().Select(s => ToplevelStatement.For(s))
            .Or(NewSymbols.Try().Select(s => ToplevelStatement.For(s)))
            .Or(BitTiming.Try().Select(s => ToplevelStatement.For(s)))
            .Or(Nodes.Try().Select(s => ToplevelStatement.For(s)))
            .Or(Message.Try().Select(s => ToplevelStatement.For(s)))
            .Or(Comment.Try().Select(s => ToplevelStatement.For(s)))
            .Or(AttributeDefinition.Try().Select(s => ToplevelStatement.For(s)))
            .Or(AttributeDefaults.Try().Select(s => ToplevelStatement.For(s)))
            .Or(AttributeValue.Try().Select(s => ToplevelStatement.For(s)))
            .Or(ValueDescription.Try().Select(s => ToplevelStatement.For(s)))
            .Or(UnknownStatement.Try().Select(s => ToplevelStatement.For(s)));

    private static TokenListParser<DbcTokens, ParsedDb> BuildDatabaseParser()
    {
        // Sections may appear in any order, any count. DefinitionEnd tokens (blank lines)
        // act as separators anywhere: leading, between statements, trailing. Unknown
        // statements (sections not modeled) are skipped but reported via
        // ParsedDb.SkippedStatements. AtEnd guarantees nothing is silently ignored.
        var statementWithSeparator =
            from ends in Token.EqualTo(DbcTokens.DefinitionEnd).Many()
            from statement in BuildStatement()
            select statement;

        return from statements in statementWithSeparator.Many()
               from ends in Token.EqualTo(DbcTokens.DefinitionEnd).Many()
               from atEnd in AtEndInternal
               select Reduce(statements);
    }

    /// <summary>Succeeds only at the true end of the token list; fails with position otherwise.</summary>
    private static readonly TokenListParser<DbcTokens, Unit> AtEndInternal =
        input =>
        {
            if (input.IsAtEnd)
                return TokenListParserResult.Value(Unit.Value, input, input);

            var next = input.ConsumeToken();
            var at = next.HasValue ? next.Remainder : input;
            return TokenListParserResult.Empty<DbcTokens, Unit>(at, "end of file");
        };

    private static ParsedDb Reduce(ToplevelStatement[] statements)
    {
        Version? version = null;
        NewSymbols? newSymbols = null;
        Nodes? nodes = null;
        var messages = new List<Message>();
        var comments = new List<CommentBase>();
        var attributeDefinitions = new List<AttributeDefinition>();
        var attributeDefaults = new List<AttributeDefault>();
        var attributeValues = new List<AttributeValue>();
        var valueDescriptions = new List<ValueDescription>();

        foreach (var s in statements)
        {
            switch (s.Kind)
            {
                case ToplevelStatement.Kinds.Version: version = s.Version; break;
                case ToplevelStatement.Kinds.NewSymbols: newSymbols = s.NewSymbols; break;
                case ToplevelStatement.Kinds.BitTiming: break;
                case ToplevelStatement.Kinds.Nodes: nodes = s.Nodes; break;
                case ToplevelStatement.Kinds.Message: messages.Add(s.Message!); break;
                case ToplevelStatement.Kinds.Comment: comments.Add(s.Comment!); break;
                case ToplevelStatement.Kinds.AttributeDefinition: attributeDefinitions.Add(s.AttributeDefinition!); break;
                case ToplevelStatement.Kinds.AttributeDefault: attributeDefaults.Add(s.AttributeDefault!); break;
                case ToplevelStatement.Kinds.AttributeValue: attributeValues.Add(s.AttributeValue!); break;
                case ToplevelStatement.Kinds.ValueDescription: valueDescriptions.Add(s.ValueDescription!); break;
            }
        }

        return new ParsedDb(
            version ?? new Version(""),
            newSymbols ?? new NewSymbols([]),
            nodes ?? new Nodes([]),
            messages.OrderBy(m => m.Header.Id).ToArray(),
            attributeDefinitions.ToArray(),
            attributeDefaults.ToArray(),
            attributeValues.ToArray(),
            valueDescriptions.ToArray(),
            comments.ToArray(),
            statements.Where(s => s.Kind == ToplevelStatement.Kinds.Unknown)
                .Select(s => s.UnknownKeyword!)
                .ToArray());
    }
}

/// <summary>Discriminated union over all top-level statement kinds, in any file order.</summary>
public sealed record ToplevelStatement(
    ToplevelStatement.Kinds Kind,
    Version? Version,
    NewSymbols? NewSymbols,
    Nodes? Nodes,
    Message? Message,
    CommentBase? Comment,
    AttributeDefinition? AttributeDefinition,
    AttributeDefault? AttributeDefault,
    AttributeValue? AttributeValue,
    ValueDescription? ValueDescription,
    string? UnknownKeyword,
    int UnknownTokenCount)
{
    public enum Kinds
    {
        Version,
        NewSymbols,
        BitTiming,
        Nodes,
        Message,
        Comment,
        AttributeDefinition,
        AttributeDefault,
        AttributeValue,
        ValueDescription,
        Unknown
    }

    public static ToplevelStatement For(Version v) => new(Kinds.Version, v, null, null, null, null, null, null, null, null, null, 0);
    public static ToplevelStatement For(NewSymbols n) => new(Kinds.NewSymbols, null, n, null, null, null, null, null, null, null, null, 0);
    public static ToplevelStatement For(Unit _) => new(Kinds.BitTiming, null, null, null, null, null, null, null, null, null, null, 0);
    public static ToplevelStatement For(Nodes n) => new(Kinds.Nodes, null, null, n, null, null, null, null, null, null, null, 0);
    public static ToplevelStatement For(Message m) => new(Kinds.Message, null, null, null, m, null, null, null, null, null, null, 0);
    public static ToplevelStatement For(CommentBase c) => new(Kinds.Comment, null, null, null, null, c, null, null, null, null, null, 0);
    public static ToplevelStatement For(AttributeDefinition a) => new(Kinds.AttributeDefinition, null, null, null, null, null, a, null, null, null, null, 0);
    public static ToplevelStatement For(AttributeDefault a) => new(Kinds.AttributeDefault, null, null, null, null, null, null, a, null, null, null, 0);
    public static ToplevelStatement For(AttributeValue a) => new(Kinds.AttributeValue, null, null, null, null, null, null, null, a, null, null, 0);
    public static ToplevelStatement For(ValueDescription v) => new(Kinds.ValueDescription, null, null, null, null, null, null, null, null, v, null, 0);
    public static ToplevelStatement For(UnknownStatement u) => new(Kinds.Unknown, null, null, null, null, null, null, null, null, null, u.Keyword, u.TokenCount);
}

/// <summary>An unmodeled section: keyword and token count (content skipped but accounted for).</summary>
public sealed record UnknownStatement(string Keyword, int TokenCount);

public sealed record ValueDescription(uint? MessageId, string? SignalName, IImmutableDictionary<long, string> Values);

public sealed record EnvironmentObjectValue(string Name, AttributeDefaultValue Value) : AttributeObjectValue(Value);

public sealed record SignalObjectValue(uint MessageId, string SignalName, AttributeDefaultValue Value)
    : AttributeObjectValue(Value);

public sealed record MessageObjectValue(uint MessageId, AttributeDefaultValue Value) : AttributeObjectValue(Value);

public sealed record NodeObjectValue(string Name, AttributeDefaultValue Value) : AttributeObjectValue(Value);

public sealed record GlobalObjectValue(AttributeDefaultValue Value) : AttributeObjectValue(Value);

public abstract record AttributeObjectValue(AttributeDefaultValue Value);

public sealed record AttributeValue(string Name, AttributeObjectValue Value);

public sealed record StringAttributeDefaultValue(string Value) : AttributeDefaultValue;

public sealed record DoubleAttributeDefaultValue(double Value) : AttributeDefaultValue;

public sealed record IntegerAttributeDefaultValue(int Value) : AttributeDefaultValue;

public abstract record AttributeDefaultValue();

public record AttributeDefault(string AttributeName, AttributeDefaultValue Value);

public record AttributeDefinition(string? ObjectType, string Name, AttributeValueType ValueType);

public sealed record EnumAttributeValueType(string[] Values) : AttributeValueType();

public sealed record StringAttributeValueType() : AttributeValueType();

public sealed record FloatAttributeValueType(double Min, double Max) : AttributeValueType()
{
    public bool IsValid => Min <= Max;
}

public sealed record HexAttributeValueType(int Min, int Max) : AttributeValueType()
{
    public bool IsValid => Min <= Max;
}

public sealed record IntAttributeValueType(int Min, int Max) : AttributeValueType()
{
    public bool IsValid => Min <= Max;
}

public abstract record AttributeValueType();

public record Multiplex(bool IsSwitch, int? SwitchValue);

public record Signal(
    string Name,
    Multiplex? Multiplex,
    int StartBit,
    int Size,
    Endianness Order,
    ValueType ValueType,
    double Factor,
    double Offset,
    double Min,
    double Max,
    string Unit,
    string Receiver);

public sealed record EnvironmentComment(string Variable, string Comment) : CommentBase(Comment);

public sealed record SignalComment(uint MessageId, string SignalName, string Comment) : CommentBase(Comment);

public sealed record MessageComment(uint MessageId, string Comment) : CommentBase(Comment);

public sealed record NodeComment(string Node, string Comment) : CommentBase(Comment);

public sealed record SimpleComment(string Comment) : CommentBase(Comment);

public abstract record CommentBase(string Comment);

public record Message(MessageHeader Header, Signal[] Signals);

public record MessageHeader(uint Id, string Name, int Size, string Node);

public record ParsedDb(
    Version Version,
    NewSymbols NewSymbols,
    Nodes Nodes,
    Message[] Messages,
    AttributeDefinition[] AttributeDefinitions,
    AttributeDefault[] AttributeDefaults,
    AttributeValue[] AttributeValues,
    ValueDescription[] ValueDescriptions,
    CommentBase[] Comments,
    string[] SkippedStatements);

public record Version(string VersionString);

public record NewSymbols(string[] NamespaceValues);

public record Nodes(string[] NodeList);

public enum ValueType
{
    Signed,
    Unsigned
}

public enum Endianness
{
    Big,
    Little
}