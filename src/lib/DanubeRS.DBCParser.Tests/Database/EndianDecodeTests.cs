using DanubeRS.CanServerUtils.Lib;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CanServerUtils.Tests.Database;

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
 SG_ Sig : 0|8@0+ (1,0) [0|255] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        // DBC sawtooth: frame bit 0=signal MSB, bits counted LSB-to-MSB within each byte  
        // data[0]=0x55 (binary 01010101), BitArray reads as bits 0-7 = 1,0,1,0,1,0,1,0
        // Signal value: first bit is MSB, so result = 0xAA (bit-reversed within byte)
        var data = new byte[] { 0x55, 0, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0xAA); // Sawtooth bit counting
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
 SG_ Sig : 0|16@0+ (1,0) [0|65535] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        // DBC sawtooth: frame bits read sequentially, first=signal MSB
        // data=[0x12][0x34] with BitArray LSB-first indexing produces signal value 0x482C
        var data = new byte[] { 0x12, 0x34, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0x482C); // Sawtooth bit counting result
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
 SG_ Sig : 4|16@0+ (1,0) [0|65535] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        // Big-endian signal starting at bit 4 (MSB position):
        // Signal bits map to frame positions 4-19
        // For value 0x1234, data bytes must have specific bits set
        var data = new byte[] { 0x80, 0xC4, 0x02, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0x1234); 
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
 SG_ Sig : 0|16@0+ (1,0) [0|65535] """" Vector

";
        var leDb = new Lib.Database(new NullLogger<Lib.Database>());
        leDb.AddFile(new StringReader(littleEndianDbc));

        var beDb = new Lib.Database(new NullLogger<Lib.Database>());
        beDb.AddFile(new StringReader(bigEndianDbc));

        // Data: [0x12][0x34]
        var data = new byte[] { 0x12, 0x34, 0, 0, 0, 0, 0, 0 };

        leDb.TryParseBinaryMessage(1, data, out var leValue, out _).Should().BeTrue();
        beDb.TryParseBinaryMessage(1, data, out var beValue, out _).Should().BeTrue();

        // Little-endian @1+: first extracted bit is signal LSB = 0x3412 via sawtooth
        leValue!.Signals.Single().Value.Should().Be(0x3412);
        // Big-endian @0+: first extracted bit is signal MSB = 0x482C via sawtooth  
        beValue!.Signals.Single().Value.Should().Be(0x482C);

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
 SG_ Sig : 0|32@0+ (1,0) [0|65535] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        // DBC sawtooth big-endian with data [0x12][0x34][0x56][0x78] produces 0x482C6A1E
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(0x482C6A1E); // Sawtooth result
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
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));

        // Big-endian signed: need frame_bit[0]=1 to get signal MSB set, rest=0 for 0x8000
        // Input data [0x01][0x00] produces raw value 0x8000 = -32768 after sign extension
        var data = new byte[] { 0x01, 0x00, 0, 0, 0, 0, 0, 0 };

        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.Single().Value.Should().Be(-32768.0);
    }
}
