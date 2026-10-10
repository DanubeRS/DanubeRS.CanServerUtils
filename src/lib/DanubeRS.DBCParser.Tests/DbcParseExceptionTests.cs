using DanubeRS.DBCParser;
using FluentAssertions;

namespace DanubeRS.DBCParser.Tests;

public class DbcParseExceptionTests
{
    [Fact]
    public void SingleError_MessageContainsCount()
    {
        var ex = new DbcParseException(["error1"]);
        // Single errors surface the positioned message verbatim (not a count wrapper).
        ex.Message.Should().Be("error1");
        ex.Errors.Should().HaveCount(1);
    }

    [Fact]
    public void MultipleErrors_MessageContainsCount()
    {
        var ex = new DbcParseException(["err1", "err2", "err3"]);
        ex.Message.Should().Contain("3 parse error(s)");
        ex.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void Errors_PreservedInOrder()
    {
        var ex = new DbcParseException(["first", "second"]);
        ex.Errors.Should().Equal("first", "second");
    }

    [Fact]
    public void SignalDecodeException_PropertiesSet()
    {
        var ex = new SignalDecodeException(0x132, "Speed", "Test message");
        ex.FrameId.Should().Be(0x132u);
        ex.SignalName.Should().Be("Speed");
        ex.Message.Should().Be("Test message");
    }

    [Fact]
    public void SignalDecodeException_NullSignalName_Allowed()
    {
        var ex = new SignalDecodeException(0x132, null, "Test message");
        ex.FrameId.Should().Be(0x132u);
        ex.SignalName.Should().BeNull();
    }
}
