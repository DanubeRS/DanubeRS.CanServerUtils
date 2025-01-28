using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace DanubeRS.CanServerUtils.Lib.DBC.Parser;

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
        from messageId in Integer
        from messageName in Token.EqualTo(DbcTokens.Identifier)
        from colon in Token.EqualTo(DbcTokens.Colon)
        from messageSize in Integer
        from nodeName in Token.EqualTo(DbcTokens.Identifier)
        select new MessageHeader(messageId, messageName.ToStringValue(),
            messageSize, nodeName.ToStringValue());

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
        Token.EqualTo(DbcTokens.Integer).Or(Token.EqualTo(DbcTokens.Double)).Select(t => int.Parse(t.ToStringValue()));

    private static TokenListParser<DbcTokens, double> Scientific { get; } =
        Token.EqualTo(DbcTokens.Sci).Select(t => double.Parse(t.ToStringValue(), NumberStyles.Float));

    private static TokenListParser<DbcTokens, double> Double { get; } = Token.EqualTo(DbcTokens.Integer)
        .Or(Token.EqualTo(DbcTokens.Double)).Select(t => double.Parse(t.ToStringValue()));

    private static TokenListParser<DbcTokens, bool> Boolean { get; } = Integer.Select(Convert.ToBoolean);

    private static TokenListParser<DbcTokens, ValueType> ValueType = Token.EqualTo(DbcTokens.Sign)
        .Select(s => s.ToStringValue() == "+" ? Parser.ValueType.Unsigned : Parser.ValueType.Signed);

    private static TokenListParser<DbcTokens, Endianness> Endianness { get; } =
        Boolean.Select(b => b ? Parser.Endianness.Little : Parser.Endianness.Big);

    private static TokenListParser<DbcTokens, string> QuotedString { get; } =
        Token.EqualTo(DbcTokens.String)
            .Select(s => s.ToStringValue());

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
        from reciever in Identifier
        select new Signal(signalName, multiplexIdentifier, startBit, signalSize, order, valueType,
            factor, offset, min, max, unit, reciever);

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
        from messageId in Integer
        from comment in QuotedString
        select (CommentBase)new MessageComment(messageId, comment);

    private static TokenListParser<DbcTokens, CommentBase> SignalComment { get; } =
        from keyword in NamedIdentifier("SG_")
        from messageId in Integer
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
        select (AttributeValueType)new IntAttributeValueType(min, max);

    private static TokenListParser<DbcTokens, AttributeValueType> HexAttributeValueType { get; } =
        from keyword in NamedIdentifier("HEX")
        from min in Integer
        from max in Integer
        select (AttributeValueType)new HexAttributeValueType(min, max);

    private static TokenListParser<DbcTokens, AttributeValueType> FloatAttributeValueType { get; } =
        from keyword in NamedIdentifier("INT")
        from min in Double
        from max in Double
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
        Token.EqualTo(DbcTokens.Integer).Select(i =>
            (AttributeDefaultValue)new IntegerAttributeDefaultValue(int.Parse(i.ToStringValue())));

    private static TokenListParser<DbcTokens, AttributeDefaultValue> DoubleAttributeDefaultValue { get; } =
        Token.EqualTo(DbcTokens.Double).Select(i =>
            (AttributeDefaultValue)new DoubleAttributeDefaultValue(double.Parse(i.ToStringValue())));

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
        from messageId in Integer
        from value in AttributeDefaultValue
        select (AttributeObjectValue)new MessageObjectValue(messageId, value);

    private static TokenListParser<DbcTokens, AttributeObjectValue> SignalObjectValue { get; } =
        from keyword in NamedIdentifier("SG_")
        from messageId in Integer
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

    private static TokenListParser<DbcTokens, ValueDescription> ValueDescription { get; } =
        from keyword in NamedIdentifier("VAL_")
        from messageId in Integer
        from signalName in Identifier
        from valueDescriptionValues in Integer
            .Then((i => QuotedString.Select(s => new KeyValuePair<uint, string>((uint)i, s)))).Many()
        from end in Token.EqualTo(DbcTokens.Semicolon)
        select new ValueDescription(messageId, signalName,
            valueDescriptionValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).ToImmutableDictionary());

    public static TokenListParser<DbcTokens, ParsedDb> Database { get; } =
        from version in Version
        from newSymbols in NewSymbols
        from bitTiming in BitTiming
        from nodes in Nodes
        from messages in Message
            .Many()
        from comments in Comment
            .Many()
        from attributeDefinitions in AttributeDefinition
            .Many()
        from attributeDefaults in AttributeDefaults
            .Many()
        from attributeValues in AttributeValue
            .Many()
        from valueDescriptions in ValueDescription
            .Many()
        select new ParsedDb(version, newSymbols, nodes, messages.OrderBy(m => m.Header.Id).ToArray(),
            attributeDefinitions, attributeDefaults, attributeValues, valueDescriptions);
}

public sealed record ValueDescription(int MessageId, string SignalName, IImmutableDictionary<uint, string> Values);

public sealed record EnvironmentObjectValue(string Name, AttributeDefaultValue Value) : AttributeObjectValue(Value);

public sealed record SignalObjectValue(int MessageId, string SignalName, AttributeDefaultValue Value)
    : AttributeObjectValue(Value);

public sealed record MessageObjectValue(int MessageId, AttributeDefaultValue Value) : AttributeObjectValue(Value);

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

public sealed record FloatAttributeValueType(double Min, double Max) : AttributeValueType();

public sealed record HexAttributeValueType(int Min, int Max) : AttributeValueType();

public sealed record IntAttributeValueType(int Min, int Max) : AttributeValueType();

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

public sealed record SignalComment(int MessageId, string SignalName, string Comment) : CommentBase(Comment);

public sealed record MessageComment(int MessageId, string Comment) : CommentBase(Comment);

public sealed record NodeComment(string Node, string Comment) : CommentBase(Comment);

public sealed record SimpleComment(string Comment) : CommentBase(Comment);

public abstract record CommentBase(string Comment);

public record Message(MessageHeader Header, Signal[] Signals);

public record MessageHeader(int Id, string Name, int Size, string Node);

public record ParsedDb(
    Version Version,
    NewSymbols NewSymbols,
    Nodes Nodes,
    Message[] Messages,
    AttributeDefinition[] AttributeDefinitions,
    AttributeDefault[] AttributeDefaults,
    AttributeValue[] AttributeValues,
    ValueDescription[] ValueDescriptions);

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