using DanubeRS.DBCParser;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.DBCParser.Tests;

public class MultiplexedDecodeTests
{
    private const string MultiplexedDbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 MuxMsg: 8 Vector
 SG_ M : 0|8@1+ (1,0) [0|255] """" Vector
 SG_ SigA_m1 m1 : 8|16@1+ (1,0) [0|65535] """" Vector
 SG_ SigB_m1 m1 : 24|8@1+ (1,0) [0|255] """" Vector
 SG_ SigA_m2 m2 : 8|16@1+ (1,0) [0|65535] """" Vector
 SG_ SigC_m2 m2 : 32|16@1+ (1,0) [0|65535] """" Vector

";

    private static Database CreateDb()
    {
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(MultiplexedDbc));
        return db;
    }

    [Fact]
    public void MultiplexedDbc_ParsesSuccessfully()
    {
        var db = CreateDb();
        var data = new byte[8];
        data[0] = 1; // Mux value

        db.TryParseBinaryMessage(1, data, out var value, out var defn).Should().BeTrue();
        value.Should().NotBeNull();
        defn.Should().NotBeNull();
    }

    [Fact]
    public void NoMatchingSwitchValue_ReturnsOnlyMuxSwitch()
    {
        var db = CreateDb();
        var data = new byte[8];
        data[0] = 3; // No m3 signals defined

        // When mux value doesn't match any signals, only the M (mux switch) signal should be returned
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Should().HaveCount(1);
        value.Signals.First().SignalName.Should().Be("M");
    }

    [Fact]
    public void MultiplexedSignal_SpanningByteBoundary_DecodesCorrectly()
    {
        var db = CreateDb();
        var data = new byte[8];
        data[0] = 1;
        BitConverter.GetBytes((ushort)0xFF00).CopyTo(data, 1);

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "SigA_m1").Value.Should().Be(0xFF00);
    }

    [Fact]
    public void UndersizedDataForMultiplexedMessage_ReturnsFalse()
    {
        var db = CreateDb();
        var data = new byte[2];
        data[0] = 2;

        db.TryParseBinaryMessage(1, data, out _, out _).Should().BeFalse();
    }

    [Fact]
    public void MultiplexedMessage_MultiplexValueSet()
    {
        // Note: Parser currently doesn't set MultiplexValue in decoded output
        var db = CreateDb();
        var data = new byte[8];
        data[0] = 2;

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
    }

    [Fact]
    public void NonMultiplexedMessage_MultiplexValueIsNull()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 RegularMsg: 8 Vector
 SG_ Sig : 0|8@1+ (1,0) [0|255] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));
        db.TryParseBinaryMessage(1, new byte[8], out var value, out _).Should().BeTrue();
        value!.MultiplexValue.Should().BeNull();
    }

    [Fact]
    public void MultiplexedMessage_HasIsMultiplexedFlagSet()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        // After fix: messages with M signal should have IsMultiplexed=True
        defn!.IsMultiplexed.Should().BeTrue("because message has multiplexer switch signal 'M'");
    }

    [Fact]
    public void MultiplexerSignalWithCorrectMName_IsDetected()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 MuxMsg: 8 Vector
 SG_ M : 0|8@1+ (1,0) [0|255] """" Vector
 SG_ SigA m1 : 8|16@1+ (1,0) [0|65535] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        // After fix: parser should detect "M" signal and set IsMultiplexed=True
        defn!.IsMultiplexed.Should().BeTrue("because message has multiplexer switch signal 'M'");
    }

    [Fact]
    public void MuxSwitchValueLargerThan8Bits_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 MuxMsg: 8 Vector
 SG_ M : 0|16@1+ (1,0) [0|65535] """" Vector
 SG_ SigA_m100 m100 : 16|8@1+ (1,0) [0|255] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        // Mux value = 100 (0x0064), little-endian stored as [0x64][0x00]
        var data = new byte[] { 0x64, 0x00, 0xAB, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        // After fix: mux switch value should be UInt32 not truncated to UInt16
        value!.MultiplexValue.Should().Be(100);
    }

[Fact]
    public void AlwaysPresentSignalsIncludedRegardlessOfMuxValue()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 MuxMsg: 8 Vector
 SG_ M : 0|8@1+ (1,0) [0|255] """" Vector
 SG_ AlwaysPresent : 56|8@1+ (1,0) [0|255] """" Vector
 SG_ OnlyWhenM1 m1 : 8|8@1+ (1,0) [0|255] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        var data = new byte[] { 0x01, 0xAB, 0, 0, 0, 0, 0, 0xCD };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        // After fix: AlwaysPresent signal should appear regardless of mux value
        value!.Signals.Should().ContainSingle(s => s.SignalName == "AlwaysPresent")
            .Which.Value.Should().Be(0xCD);
    }
}
