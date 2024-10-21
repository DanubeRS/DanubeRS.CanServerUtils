using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;

public static class DbcTokenizer
{
    private static TextParser<Unit> StringValue { get; } =
        from start in Character.EqualTo('"')
        from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
            .Or(Character.Except('"').Value(Unit.Value))
            .IgnoreMany()
        from end in Character.EqualTo('"')
        select Unit.Value;

    public static Tokenizer<DbcTokens> Instance =>
        new TokenizerBuilder<DbcTokens>()
            .Match(Span.EqualTo("\n\n"), DbcTokens.DefinitionEnd)
            .Ignore(Character.WhiteSpace)
            .Match(Character.EqualTo(':'), DbcTokens.Colon)
            .Match(Identifier.CStyle, DbcTokens.Identifier)
            .Match(StringValue, DbcTokens.String)
            .Build();
}