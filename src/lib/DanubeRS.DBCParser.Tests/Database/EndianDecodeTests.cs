using DanubeRS.DBCParser;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.DBCParser.Tests;

public class EndianDecodeTests
{
    [Fact]
    public void BigEndian8BitSignal_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 7|8@0+ (1,0) [0|255] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        // DBC sawtooth: frame bit 0=signal MSB, bits counted LSB-to-MSB within each byte  
        // data[0]=0x55 (binary 01010101), BitArray reads as bits 0-7 = 1,0,1,0,1,0,1,0
        // Signal value: first bit is MSB, so result = 0xAA (bit-reversed within byte)
        var data = new byte[] { 0x55, 0, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0x55); // Spec sawtooth: start 7 = MSB; byte value unchanged
    }

    [Fact]
    public void BigEndian16BitSignal_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 7|16@0+ (1,0) [0|65535] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        // Spec sawtooth: start bit 7 is the signal MSB; bits descend 7..0 then continue at 15..8.
        // data=[0x12][0x34] therefore decodes as 0x1234.
        var data = new byte[] { 0x12, 0x34, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0x1234); // Spec sawtooth result
    }

    [Fact]
    public void BigEndian16BitSignalAtNonByteAlignedOffset_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 15|16@0+ (1,0) [0|65535] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        // Big-endian signal starting at bit 15 (MSB of byte 1): bits descend 15..8 then 23..16.
        // data=[0x80][0xC4][0x02] decodes as 0xC402.
        var data = new byte[] { 0x80, 0xC4, 0x02, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0xC402);
    }

    [Fact]
    public void LittleEndianVsBigEndian_ProducesDifferentResults()
    {
        const string littleEndianDbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 0|16@1+ (1,0) [0|65535] """" Vector

";
        const string bigEndianDbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 7|16@0+ (1,0) [0|65535] """" Vector

";
        var leDb = new Database(new NullLogger<Database>());
        leDb.AddFile(new StringReader(littleEndianDbc));

        var beDb = new Database(new NullLogger<Database>());
        beDb.AddFile(new StringReader(bigEndianDbc));

        // Data: [0x12][0x34]
        var data = new byte[] { 0x12, 0x34, 0, 0, 0, 0, 0, 0 };

        leDb.TryParseBinaryMessage(1, data, out var leValue, out _).Should().BeTrue();
        beDb.TryParseBinaryMessage(1, data, out var beValue, out _).Should().BeTrue();

        // Little-endian @1+: first extracted bit is signal LSB = 0x3412 via sawtooth
        leValue!.Signals.Single().Value.Should().Be(0x3412);
        // Big-endian @0+ (start 7): spec sawtooth MSB-first = 0x1234
        beValue!.Signals.Single().Value.Should().Be(0x1234);

        // THESE MUST BE DIFFERENT - if they're equal, endianness bug exists!
        leValue.Signals.Single().Value.Should().NotBe(beValue.Signals.Single().Value);
    }

    [Fact]
    public void BigEndian32BitSignal_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 7|32@0+ (1,0) [0|65535] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        // Spec sawtooth big-endian (start 7) with data [0x12][0x34][0x56][0x78] produces 0x12345678
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0x12345678); // Spec sawtooth result
    }

    [Fact]
    public void BigEndianSignedSignal_NegativeValue_DecodesCorrectly()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Sig : 0|16@0- (1,0) [0|65535] """" Vector

";
        var db = new Database(new NullLogger<Database>());
        db.AddFile(new StringReader(dbc));

        // Big-endian signed: need frame_bit[0]=1 to get signal MSB set, rest=0 for 0x8000
        // Input data [0x01][0x00] produces raw value 0x8000 = -32768 after sign extension
        var data = new byte[] { 0x01, 0x00, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(-32768.0);
    }
}
