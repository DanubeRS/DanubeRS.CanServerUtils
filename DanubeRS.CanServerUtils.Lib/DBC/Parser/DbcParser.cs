using System.Reflection.Metadata;
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
        from buKeyword in Token.EqualToValue(DbcTokens.Identifier, "BU_").Message("BU")
        from colon in Token.EqualTo(DbcTokens.Colon)
        from nodes in Token.EqualTo(DbcTokens.Identifier)
            .Many()
        // from end in Token.EqualTo(DbcTokens.DefinitionEnd)
        select new Nodes(nodes.Select(t => t.ToStringValue()).ToArray());

    public static TokenListParser<DbcTokens, ParsedDb> Database { get; } =
        from version in Version
        from newSymbols in NewSymbols
        from bitTiming in BitTiming
        from nodes in Nodes
            .AtEnd()
        select new ParsedDb(version, newSymbols, nodes);
}

public record ParsedDb(Version Version, NewSymbols NewSymbols, Nodes nodes);

public record Version(string VersionString);

public record NewSymbols(string[] NamespaceValues);

public record Nodes(string[] nodes);