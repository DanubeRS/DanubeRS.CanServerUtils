using Superpower.Display;

namespace DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;

public enum DbcTokens
{
    [Token(Category = "keyword", Example = "NS_")]
    Keyword,
    [Token(Category = "grammar", Example = ":")]
    Colon,
    [Token(Category = "grammar", Example = "[")]
    LSquareBracket,
    [Token(Category = "grammar", Example = "]")]
    RSquareBracket,
    [Token(Category = "grammar", Example = ",")]
    Comma,
    [Token(Category = "identifier", Example = "CS_")]
    Identifier,
    [Token(Category = "value", Example = "test")]
    String,
    [Token(Category = "grammar")]
    Newline
}