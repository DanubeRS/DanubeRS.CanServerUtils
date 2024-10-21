using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace DanubeRS.CanServerUtils.Lib.DBC.Parser;

public static class DbcParser
{
    private static TokenListParser<DbcTokens, Version> Version { get; } =
        from versionKeyword in Token.EqualToValue(DbcTokens.Identifier, "VERSION")
        from version in Token.EqualTo(DbcTokens.String)
        from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select new Version(version.ToStringValue());

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
        from messageId in Token.EqualTo(DbcTokens.Integer)
        from messageName in Token.EqualTo(DbcTokens.Identifier)
        from colon in Token.EqualTo(DbcTokens.Colon)
        from messageSize in Token.EqualTo(DbcTokens.Integer)
        from nodeName in Token.EqualTo(DbcTokens.Identifier)
        select new MessageHeader(int.Parse(messageId.ToStringValue()), messageName.ToStringValue(), int.Parse(messageSize.ToStringValue()), nodeName.ToStringValue());
    
    // If has value, then it is a multiplexed signal, if -1, it is the multiplexer
    private static TokenListParser<DbcTokens, Multiplex> Multiplex { get; } =
        from sw in Token.EqualToValue(DbcTokens.Identifier, "m").Or(Token.EqualToValue(DbcTokens.Identifier, "M"))
        from value in Token.EqualTo(DbcTokens.Integer)
        select sw.ToStringValue() == "m"
            ? new Multiplex(true, int.Parse(value.ToStringValue()))
            : new Multiplex(false, null);
    

    private static TokenListParser<DbcTokens, int> Integer { get; } = Token.EqualTo(DbcTokens.Integer).Select(t => int.Parse(t.ToStringValue()));
    private static TokenListParser<DbcTokens, double> Double { get; } = Token.EqualTo(DbcTokens.Integer).Or(Token.EqualTo(DbcTokens.Double)).Select(t => double.Parse(t.ToStringValue()));
    private static TokenListParser<DbcTokens, bool> Bool { get; } = Integer.Select(Convert.ToBoolean);

    private static TokenListParser<DbcTokens, ValueType> ValueType = Token.EqualTo(DbcTokens.Sign)
        .Select(s => s.ToStringValue() == "+" ? Parser.ValueType.Unsigned : Parser.ValueType.Signed);
    private static TokenListParser<DbcTokens, Endianness> Endianness { get; } = Token.EqualToValue(DbcTokens.Integer, "0").Or(Token.EqualToValue(DbcTokens.Integer, "1")).Select(t => t.ToStringValue() == "0" ? Parser.Endianness.Big : Parser.Endianness.Little);
    private static TokenListParser<DbcTokens, string> StringValue { get; } = Token.EqualTo(DbcTokens.String).Select(s => s.ToStringValue());
    private static TokenListParser<DbcTokens, Signal> Signal { get; } =
        from sgKeyword in Token.EqualToValue(DbcTokens.Identifier, "SG_")
        from signalName in Token.EqualTo(DbcTokens.Identifier)
        from multiplexIdentifier in Multiplex.OptionalOrDefault()
        from colon in Token.EqualTo(DbcTokens.Colon)
        from startBit in Integer
        from pipe in Token.EqualTo(DbcTokens.Pipe)
        from signalSize in Integer
        from orderDelim in Token.EqualTo(DbcTokens.AtSymbol)
        from order in Endianness
        from valueType in ValueType
        from lParen in Token.EqualTo(DbcTokens.LParen)
        from factor in Double
        from factorOffsetComma in Token.EqualTo(DbcTokens.Comma)
        from offset in Double
        from rParen in Token.EqualTo(DbcTokens.RParen)
        from lSquParen in Token.EqualTo(DbcTokens.LSqParen)
        from min in Double
        from minMaxPipe in Token.EqualTo(DbcTokens.Pipe)
        from max in Double
        from rSquParen in Token.EqualTo(DbcTokens.RSqParen)
        from unit in StringValue
        from reciever in Token.EqualTo(DbcTokens.Identifier)
        select new Signal(signalName.ToStringValue(), multiplexIdentifier, startBit, signalSize, order, valueType, factor, offset, min, max, unit, reciever.ToStringValue());

    private static TokenListParser<DbcTokens, Message> Message { get; } =
        from header in MessageHeader
        from signals in Signal.Many()
        select new Message(header, signals);

    public static TokenListParser<DbcTokens, ParsedDb> Database { get; } =
        from version in Version
        from newSymbols in NewSymbols
        from bitTiming in BitTiming
        from nodes in Nodes
        from messages in Message
            .Many()
            .AtEnd()
        select new ParsedDb(version, newSymbols, nodes, messages);
}

public record Multiplex(bool IsSwitch, int? SwitchValue);

public record Signal(
    string name,
    Multiplex multiplex,
    int startBit,
    int signalSize,
    Endianness order,
    ValueType valueType,
    double factor,
    double offset,
    double min,
    double max,
    string unit,
    string reciever);

public record Message(MessageHeader Header, Signal[] Signals);
public record MessageHeader(int Id, string Name, int Size, string Node);

public record ParsedDb(Version Version, NewSymbols NewSymbols, Nodes Nodes, Message[] messages);

public record Version(string VersionString);

public record NewSymbols(string[] NamespaceValues);

public record Nodes(string[] NodeList);

public enum ValueType
{
    Signed, Unsigned
}

public enum Endianness 
{
    Big,
    Little
}