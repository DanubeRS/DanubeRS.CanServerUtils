using DanubeRS.CanServerUtils.Lib;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class ExtendedIdDecodeTests
{
    [Fact]
    public void ExtendedCanFrame_DecodesWithFull32BitId()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 536870912 ExtMsg: 8 Vector
 SG_ Data : 0|8@1+ (1,0) [0|255] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        // Extended CAN ID: 0x20000000
        var data = new byte[] { 0xAB, 0, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(536870912u, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0xAB);
    }

    [Fact]
    public void MaximumExtendedCanId_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 4294967295 MaxExtMsg: 8 Vector
 SG_ Data : 0|8@1+ (1,0) [0|255] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        db.TryParseBinaryMessage(4294967295u, new byte[] { 0x12, 0, 0, 0, 0, 0, 0, 0 }, 
            out var value, out _).Should().BeTrue();
    }
}
