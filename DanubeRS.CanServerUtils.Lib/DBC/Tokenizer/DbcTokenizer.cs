using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;

public static class DbcTokenizer
{
    private static TextParser<Unit> StringValue =>
        from start in Character.EqualTo('"')
        from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
            .Or(Character.Except('"').Value(Unit.Value))
            .IgnoreMany()
        from end in Character.EqualTo('"')
        select Unit.Value;

    public static Tokenizer<DbcTokens> Instance => 
        new TokenizerBuilder<DbcTokens>()
        .Match(Span.Regex("[A-Za-z0-9_]+"), DbcTokens.Keyword)
        .Match(Character.EqualTo(':'), DbcTokens.Colon)
        .Match(Character.EqualTo('['), DbcTokens.LSquareBracket)
        .Match(Character.EqualTo(']'), DbcTokens.RSquareBracket)
        // .Match(Character.EqualTo('\n').Or(Character.EqualTo('\r')), DbcTokens.Newline)
        .Match(StringValue, DbcTokens.String)
        // .Match(Span.Regex("[A-Za-z0-9_]+]"), DbcTokens.Identifier)
        .Ignore(Character.WhiteSpace)
        .Build();
}