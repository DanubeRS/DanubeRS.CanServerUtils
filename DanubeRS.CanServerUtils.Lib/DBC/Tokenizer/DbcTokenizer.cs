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
            .Match(Span.Regex("[\n]{2,}"), DbcTokens.DefinitionEnd)
            .Match(Numerics.Integer, DbcTokens.Integer)
            .Match(Numerics.DecimalDouble, DbcTokens.Double)
            .Ignore(Character.WhiteSpace)
            .Match(Character.EqualTo(':'), DbcTokens.Colon)
            .Match(Identifier.CStyle, DbcTokens.Identifier)
            .Match(StringValue, DbcTokens.String)
            .Match(Character.EqualTo('('), DbcTokens.LParen)
            .Match(Character.EqualTo(')'), DbcTokens.RParen)
            .Match(Character.EqualTo('['), DbcTokens.LSqParen)
            .Match(Character.EqualTo(']'), DbcTokens.RSqParen)
            .Match(Character.EqualTo('@'), DbcTokens.AtSymbol)
            .Match(Character.EqualTo('|'), DbcTokens.Pipe)
            .Match(Character.EqualTo(','), DbcTokens.Comma)
            .Match(Character.EqualTo('+').Or(Character.EqualTo('-')), DbcTokens.Sign)
            .Build();
}