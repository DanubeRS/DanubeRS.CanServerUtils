using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class DecodeTests
{
    //TODO 
    [Fact]
    public void ValidPacketData_Parsed_ValidOutputData()
    {
        var frameData = new byte[]{0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08};
        const byte frameId = 0x01;

        var resultSuccess = DatabaseInstance.TryParseBinaryMessage(frameId, frameData, out var results);

        resultSuccess.Should().BeTrue();
        results.Should().NotBeNull();
        results!.MessageId.Should().Be(frameId);
        results.Signals.Should().HaveCount(1);

    }

    private Lib.DBC.Database DatabaseInstance => new Lib.DBC.Database(new NullLogger<Lib.DBC.Database>());
}