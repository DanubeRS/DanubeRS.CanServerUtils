using Superpower.Display;

namespace DanubeRS.CanServerUtils.Lib.DBC.Tokenizer;

public enum DbcTokens
{
    [Token(Category = "grammar", Example = ":")]
    Colon,

    [Token(Category = "identifier", Example = "CS_")]
    Identifier,

    [Token(Category = "value", Example = "test")]
    String,

    [Token(Category = "grammar")] DefinitionEnd,
    [Token(Category = "value", Example = "123")]
    Integer,
    [Token(Category = "value", Example = "123.45")]
    Double,
    [Token(Category = "grammar", Example = "|")]
    Pipe,
    [Token(Category = "grammar", Example = "@")]
    AtSymbol,
    [Token(Category = "grammar", Example = "+, -")]
    Sign,
    [Token(Category = "grammar", Example = "(")]
    LParen,
    [Token(Category = "grammar", Example = ")")]
    RParen,
    [Token(Category = "grammar", Example = "[")]
    LSqParen,
    [Token(Category = "grammar", Example = "]")]
    RSqParen,
    [Token(Category = "grammar", Example = ",")]
    Comma
}