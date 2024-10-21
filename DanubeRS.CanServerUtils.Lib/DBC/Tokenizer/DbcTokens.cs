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

    [Token(Category = "grammar")] DefinitionEnd
}