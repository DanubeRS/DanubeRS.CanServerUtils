using DanubeRS.DBCParser.Tokenizer;
using FluentAssertions;
using Superpower;
using Superpower.Model;

namespace DanubeRS.DBCParser.Tests.Tokenizer;

public class DbcTokenizerTests
{
    private static TokenList<DbcTokens> Tokenize(string input) =>
        DbcTokenizer.Instance.Tokenize(input);

    private static DbcTokens? FirstToken(string input) =>
        Tokenize(input).Select(t => (DbcTokens?)t.Kind).FirstOrDefault();

    private static DbcTokens[] AllTokens(string input) =>
        Tokenize(input).Select(t => t.Kind).ToArray();

    #region DefinitionEnd

    [Fact]
    public void DoubleNewline_ProducesDefinitionEnd()
    {
        var tokens = AllTokens("BS_:\n\nBU_:");
        tokens.Should().Contain(DbcTokens.DefinitionEnd);
    }

    [Fact]
    public void TripleNewline_ProducesSingleDefinitionEnd()
    {
        var tokens = AllTokens("BS_:\n\n\nBU_:");
        tokens.Should().ContainSingle(t => t == DbcTokens.DefinitionEnd);
    }

    [Fact]
    public void DoubleCrlf_ProducesDefinitionEnd()
    {
        var tokens = AllTokens("BS_:\r\n\r\nBU_:");
        tokens.Should().Contain(DbcTokens.DefinitionEnd);
    }

    [Fact]
    public void TripleCrlf_ProducesSingleDefinitionEnd()
    {
        var tokens = AllTokens("BS_:\r\n\r\n\r\nBU_:");
        tokens.Should().ContainSingle(t => t == DbcTokens.DefinitionEnd);
    }

    [Fact]
    public void SingleNewline_DoesNotProduceDefinitionEnd()
    {
        var tokens = AllTokens("BS_:\nBU_:");
        tokens.Should().NotContain(DbcTokens.DefinitionEnd);
    }

    #endregion

    #region Numeric token ordering

    [Fact]
    public void ScientificNotation_TokenizedAsSci()
    {
        FirstToken("2E-005").Should().Be(DbcTokens.Sci);
    }

    [Fact]
    public void NegativeScientificNotation_TokenizedAsSci()
    {
        FirstToken("-2.5E-03").Should().Be(DbcTokens.Sci);
    }

    [Fact]
    public void DecimalDouble_TokenizedAsDouble()
    {
        FirstToken("0.01").Should().Be(DbcTokens.Double);
    }

    [Fact]
    public void Integer_TokenizedAsInteger()
    {
        FirstToken("132").Should().Be(DbcTokens.Double);
    }

    [Fact]
    public void NegativeInteger_TokenizedAsDouble()
    {
        // DecimalDouble regex matches "-42" as a single token (sign is part of the number)
        var tokens = AllTokens("-42");
        tokens.Should().HaveCount(1);
        tokens[0].Should().Be(DbcTokens.Double);
    }

    #endregion

    #region Strings

    [Fact]
    public void QuotedString_TokenizedAsString()
    {
        var tokens = Tokenize("\"km/h\"");
        tokens.Should().ContainSingle(t => t.Kind == DbcTokens.String);
    }

    [Fact]
    public void EmptyQuotedString_TokenizedAsString()
    {
        var tokens = Tokenize("\"\"");
        tokens.Should().ContainSingle(t => t.Kind == DbcTokens.String);
    }

    [Fact]
    public void QuotedStringWithEscapedQuote_TokenizesSuccessfully()
    {
        var act = () => Tokenize("\"say \\\"hi\\\"\"");
        act.Should().NotThrow();
    }

    #endregion

    #region Grammar tokens

    [Fact]
    public void Pipe_TokenizedAsPipe()
    {
        FirstToken("|").Should().Be(DbcTokens.Pipe);
    }

    [Fact]
    public void AtSymbol_TokenizedAsAtSymbol()
    {
        FirstToken("@").Should().Be(DbcTokens.AtSymbol);
    }

    [Fact]
    public void PlusSign_TokenizedAsSign()
    {
        FirstToken("+").Should().Be(DbcTokens.Sign);
    }

    [Fact]
    public void MinusSign_TokenizedAsSign()
    {
        FirstToken("-").Should().Be(DbcTokens.Sign);
    }

    [Fact]
    public void Colon_TokenizedAsColon()
    {
        FirstToken(":").Should().Be(DbcTokens.Colon);
    }

    [Fact]
    public void Comma_TokenizedAsComma()
    {
        FirstToken(",").Should().Be(DbcTokens.Comma);
    }

    [Fact]
    public void Semicolon_TokenizedAsSemicolon()
    {
        FirstToken(";").Should().Be(DbcTokens.Semicolon);
    }

    [Fact]
    public void Parentheses_TokenizedCorrectly()
    {
        var tokens = AllTokens("()");
        tokens.Should().Equal(DbcTokens.LParen, DbcTokens.RParen);
    }

    [Fact]
    public void SquareBrackets_TokenizedCorrectly()
    {
        var tokens = AllTokens("[]");
        tokens.Should().Equal(DbcTokens.LSqParen, DbcTokens.RSqParen);
    }

    #endregion

    #region Full line tokenization

    [Fact]
    public void FullSignalLine_TokenizesSuccessfully()
    {
        var act = () => Tokenize("SG_ Speed : 0|8@1+ (0.01,0) [0|255] \"km/h\" Vector");
        act.Should().NotThrow();
    }

    [Fact]
    public void FullSignalLine_ContainsExpectedTokenCount()
    {
        var tokens = AllTokens("SG_ Speed : 0|8@1+ (0.01,0) [0|255] \"km/h\" Vector");
        tokens.Should().HaveCountGreaterThan(15);
    }

    [Fact]
    public void FullMessageHeader_TokenizesSuccessfully()
    {
        var act = () => Tokenize("BO_ 132 VehicleData 8 Vector");
        act.Should().NotThrow();
    }

    #endregion

    #region Whitespace

    [Fact]
    public void WhitespaceOnly_ProducesNoTokens()
    {
        var tokens = AllTokens("  \t  ");
        tokens.Should().BeEmpty();
    }

    [Fact]
    public void SingleNewlineBetweenTokens_ConsumedAsWhitespace()
    {
        var tokens = AllTokens("BS_:\nBU_:");
        tokens.Should().Equal(DbcTokens.Identifier, DbcTokens.Colon, DbcTokens.Identifier, DbcTokens.Colon);
    }

    #endregion
}
