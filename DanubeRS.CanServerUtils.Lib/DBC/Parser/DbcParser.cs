using System.Reflection.Metadata;
using DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;
using Superpower;
using Superpower.Parsers;

namespace DanubeRS.CanServerUtils.Lib.DBC.Parser;

public static class DbcParser
{


    public static TokenListParser<DbcTokens, object> Version { get; } =
        from versionKeyword in Token.EqualToValue(DbcTokens.Keyword, "VERSION")
        from version in Token.EqualTo(DbcTokens.String)
        select (object)new Version(version.ToStringValue());
        
    public static TokenListParser<DbcTokens, object> Namespace { get; } =
        from nsKeyword in Token.EqualToValue(DbcTokens.Keyword, "NS_")
        from _ in Token.EqualTo(DbcTokens.Colon)
        from values in Token.EqualTo(DbcTokens.Keyword)
            .Many()
        select (object)new Namespace(values.Select(v => v.ToStringValue()).ToArray());

    public static TokenListParser<DbcTokens, object[]> Database { get; } =
        Version.Or(Namespace).Many().AtEnd();
}

public record ParsedDb(Namespace Namespace, Version Version);
public record Namespace(string[] NamespaceValues);
public record Version(string VersionString);