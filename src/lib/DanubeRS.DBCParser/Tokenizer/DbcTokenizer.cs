using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace DanubeRS.CanServerUtils.Lib.Tokenizer;

public static class DbcTokenizer
{
    static TextParser<Unit> StringValue { get; } =
        from open in Character.EqualTo('"')
        from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
            .Or(Character.Except('"').Value(Unit.Value))
            .IgnoreMany()
        from close in Character.EqualTo('"')
        select Unit.Value;

    public static Tokenizer<DbcTokens> Instance =>
        new TokenizerBuilder<DbcTokens>()
            .Match(Span.Regex("[\n]{2,}"), DbcTokens.DefinitionEnd)
            .Match(Span.Regex("(\r\n){2,}"), DbcTokens.DefinitionEnd)
            .Match(Span.Regex(@"([-+]?[0-9]*\.?[0-9])+[eE]([-+]?[0-9]+)?"), DbcTokens.Sci)
            .Match(Numerics.DecimalDouble, DbcTokens.Double)
            .Match(Numerics.Integer, DbcTokens.Integer)
            .Match(StringValue, DbcTokens.String)
            .Match(Identifier.CStyle, DbcTokens.Identifier)
            .Match(Character.EqualTo('|'), DbcTokens.Pipe)
            .Ignore(Character.WhiteSpace)
            .Match(Character.EqualTo('\"'), DbcTokens.Quote)
            .Match(Character.EqualTo(':'), DbcTokens.Colon)
            .Match(Character.EqualTo('('), DbcTokens.LParen)
            .Match(Character.EqualTo(')'), DbcTokens.RParen)
            .Match(Character.EqualTo('['), DbcTokens.LSqParen)
            .Match(Character.EqualTo(']'), DbcTokens.RSqParen)
            .Match(Character.EqualTo('@'), DbcTokens.AtSymbol)
            .Match(Character.EqualTo(','), DbcTokens.Comma)
            .Match(Character.EqualTo('+').Or(Character.EqualTo('-')), DbcTokens.Sign)
            .Match(Character.EqualTo(';'), DbcTokens.Semicolon)
            .Build();
}