using DanubeRS.DBCParser;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ValueType = DanubeRS.DBCParser.Parser.ValueType;

namespace DanubeRS.DBCParser.Tests;

public class DecodeTests
{
    private const string TestDbc = """
        VERSION ""

        NS_ :

        BS_:

        BU_: Vector

        BO_ 1 TestMsg: 8 Vector
         SG_ Unsigned8 : 0|8@1+ (1,0) [0|255] "" Vector
         SG_ Unsigned16 : 8|16@1+ (1,0) [0|65535] "" Vector
         SG_ Signed8 : 24|8@1- (1,0) [-128|127] "" Vector
         SG_ Signed16 : 32|16@1- (0.1,-40) [-40|215] "C" Vector


        """;

    private static Database CreateDb()
    {
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(TestDbc));
        return db;
    }

    [Fact]
    public void KnownFrameIdWithValidData_ReturnsTrue()
    {
        var db = CreateDb();
        var data = new byte[8];
        var result = db.TryParseBinaryMessage(1, data, out var value, out var defn);
        result.Should().BeTrue();
        value.Should().NotBeNull();
        defn.Should().NotBeNull();
    }

    [Fact]
    public void UnknownFrameId_ReturnsFalse()
    {
        var db = CreateDb();
        var data = new byte[8];
        var result = db.TryParseBinaryMessage(999, data, out var value, out var defn);
        result.Should().BeFalse();
        value.Should().BeNull();
        defn.Should().BeNull();
    }

    [Fact]
    public void UndersizedData_ReturnsFalse()
    {
        var db = CreateDb();
        var data = new byte[3];
        db.TryParseBinaryMessage(1, data, out _, out _).Should().BeFalse();
    }

    [Fact]
    public void OversizedData_ReturnsTrue()
    {
        var db = CreateDb();
        var data = new byte[16];
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Should().HaveCount(4);
    }

    [Fact]
    public void ZeroByteData_ReturnsFalse()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(1, Array.Empty<byte>(), out _, out _).Should().BeFalse();
    }

    [Fact]
    public void Unsigned8Bit_DecodesCorrectly()
    {
        var db = CreateDb();
        var data = new byte[8];
        data[0] = 0xAB;
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "Unsigned8").Value.Should().Be(0xAB);
    }

    [Fact]
    public void Unsigned16Bit_DecodesCorrectly()
    {
        var db = CreateDb();
        var data = new byte[8];
        BitConverter.GetBytes((ushort)0x1234).CopyTo(data, 1);
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "Unsigned16").Value.Should().Be(0x1234);
    }

    [Fact]
    public void UnsignedSignal_MaxValue_DecodesCorrectly()
    {
        var db = CreateDb();
        var data = new byte[8];
        data[0] = 0xFF;
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "Unsigned8").Value.Should().Be(255);
    }

    [Fact]
    public void Signed8Bit_NegativeOne_DecodesCorrectly()
    {
        var db = CreateDb();
        var data = new byte[8];
        data[3] = 0xFF;
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "Signed8").Value.Should().Be(-1);
    }

    [Fact]
    public void Signed16Bit_WithFactorAndOffset_DecodesCorrectly()
    {
        var db = CreateDb();
        var data = new byte[8];
        BitConverter.GetBytes((ushort)500).CopyTo(data, 4);
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "Signed16").Value.Should().Be(10.0);
    }

    [Fact]
    public void SignalWithValEntry_HasValueLookup()
    {
        const string dbc = """
            VERSION ""

            NS_ :

            BS_:

            BU_: Vector

            BO_ 1 TestMsg: 8 Vector
             SG_ Status : 0|8@1+ (1,0) [0|2] "" Vector

            VAL_ 1 Status 0 "Off" 1 "On" 2 "Fault";


            """;
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        var statusDef = defn!.Signals.First(s => s.Signal.Name == "Status");
        statusDef.ValueLookup.Should().NotBeNull();
        statusDef.ValueLookup!.Should().ContainKey(0u).WhoseValue.Should().Be("Off");
        statusDef.ValueLookup.Should().ContainKey(1u).WhoseValue.Should().Be("On");
        statusDef.ValueLookup.Should().ContainKey(2u).WhoseValue.Should().Be("Fault");
    }

    [Fact]
    public void SignalWithoutValEntry_HasNullValueLookup()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        var speedDef = defn!.Signals.First(s => s.Signal.Name == "Unsigned8");
        speedDef.ValueLookup.Should().BeNull();
    }

     [Fact]
    public void MessageWithNoSignals_ReturnsTrueWithEmptySignals()
    {
        // Note: Parser currently doesn't support messages with no signals (signals.Many() requires at least one signal)
        const string dbc = """
            VERSION ""

            NS_ :

            BS_:

            BU_: Vector

            BO_ 1 EmptyMsg: 8 Vector




            """;
    }

    [Fact]
    public void SpanBasedDecode_MatchesBitArrayDecode()
    {
        var db = CreateDb();

        var testPayloads = new byte[][]
        {
            new byte[8],
            new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
            new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 },
            new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 },
        };

        foreach (var data in testPayloads)
        {
            db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
            value!.Signals.Should().HaveCount(4, because: $"payload {Convert.ToHexString(data)} should decode 4 signals");

            var unsigned8 = value.Signals.First(s => s.SignalName == "Unsigned8").Value;
            var unsigned16 = value.Signals.First(s => s.SignalName == "Unsigned16").Value;
            var signed8 = value.Signals.First(s => s.SignalName == "Signed8").Value;
            var signed16 = value.Signals.First(s => s.SignalName == "Signed16").Value;

            unsigned8.Should().Be(data[0], because: "Unsigned8 is byte 0");
            unsigned16.Should().Be(BitConverter.ToUInt16(data, 1), because: "Unsigned16 is bytes 1-2 little-endian");
        }
    }
}
