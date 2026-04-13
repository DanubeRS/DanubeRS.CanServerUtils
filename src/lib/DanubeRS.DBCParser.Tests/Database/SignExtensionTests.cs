using DanubeRS.CanServerUtils.Lib;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class SignExtensionTests
{
    private static Lib.Database CreateDbWithSignedSignal(int startBit, int size, double factor = 1, double offset = 0)
    {
        var dbc = $"VERSION \"\"\n\nNS_ :\n\nBS_:\n\nBU_: Vector\n\nBO_ 1 TestMsg: 8 Vector\n SG_ SignedSignal : {startBit}|{size}@1- ({factor},{offset}) [0|0] \"\" Vector\n\n";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));
        return db;
    }

    private static double DecodeSignedValue(Lib.Database db, byte[] data)
    {
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        return value!.Signals.First(s => s.SignalName == "SignedSignal").Value;
    }

    [Theory]
    [InlineData((byte)0x00, 0L)]
    [InlineData((byte)0x7F, 127L)]
    [InlineData((byte)0x80, -128L)]
    [InlineData((byte)0xFF, -1L)]
    public void Signed8Bit_BoundaryValues_DecodeCorrectly(byte rawValue, long expected)
    {
        var db = CreateDbWithSignedSignal(0, 8);
        var data = new byte[8];
        data[0] = rawValue;
        DecodeSignedValue(db, data).Should().Be(expected);
    }

    [Theory]
    [InlineData((ushort)0x0000, 0L)]
    [InlineData((ushort)0x7FFF, 32767L)]
    [InlineData((ushort)0x8000, -32768L)]
    [InlineData((ushort)0xFFFF, -1L)]
    public void Signed16Bit_BoundaryValues_DecodeCorrectly(ushort rawValue, long expected)
    {
        var db = CreateDbWithSignedSignal(0, 16);
        var data = new byte[8];
        BitConverter.GetBytes(rawValue).CopyTo(data, 0);
        DecodeSignedValue(db, data).Should().Be(expected);
    }

    [Theory]
    [InlineData(0x00000000u, 0L)]
    [InlineData(0x7FFFFFFFu, 2147483647L)]
    [InlineData(0x80000000u, -2147483648L)]
    [InlineData(0xFFFFFFFFu, -1L)]
    public void Signed32Bit_BoundaryValues_DecodeCorrectly(uint rawValue, long expected)
    {
        var db = CreateDbWithSignedSignal(0, 32);
        var data = new byte[8];
        BitConverter.GetBytes(rawValue).CopyTo(data, 0);
        DecodeSignedValue(db, data).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 0L)]
    [InlineData(1, -1L)]
    public void Signed1Bit_Values_DecodeCorrectly(int bitValue, long expected)
    {
        var db = CreateDbWithSignedSignal(0, 1);
        var data = new byte[8];
        if (bitValue == 1) data[0] = 0x01;
        DecodeSignedValue(db, data).Should().Be(expected);
    }

    [Fact]
    public void Signed8Bit_AtNonZeroStartBit_DecodesCorrectly()
    {
        var db = CreateDbWithSignedSignal(8, 8);
        var data = new byte[8];
        data[1] = 0xFF;
        DecodeSignedValue(db, data).Should().Be(-1);
    }

    [Fact]
    public void Signed16Bit_AtNonZeroStartBit_DecodesCorrectly()
    {
        var db = CreateDbWithSignedSignal(16, 16);
        var data = new byte[8];
        BitConverter.GetBytes((ushort)0xFFFF).CopyTo(data, 2);
        DecodeSignedValue(db, data).Should().Be(-1);
    }

    [Fact]
    public void SignedSignal_WithFactorAndOffset_AppliesCorrectly()
    {
        var db = CreateDbWithSignedSignal(0, 8, factor: 0.1, offset: 100);
        var data = new byte[8];
        data[0] = 0xFF;
        DecodeSignedValue(db, data).Should().BeApproximately(99.9, 0.001);
    }

    [Fact]
    public void SignedSignal_NegativeFactor_AppliesCorrectly()
    {
        var db = CreateDbWithSignedSignal(0, 8, factor: -0.1, offset: 0);
        var data = new byte[8];
        data[0] = 0x64;
        DecodeSignedValue(db, data).Should().Be(-10.0);
    }

    [Fact]
    public void SignedSignal_NegativeOffset_AppliesCorrectly()
    {
        var db = CreateDbWithSignedSignal(0, 8, factor: 1, offset: -128);
        var data = new byte[8];
        data[0] = 0x00;
        DecodeSignedValue(db, data).Should().Be(-128.0);
    }

    [Theory]
    [InlineData((byte)0x01, 1L)]
    [InlineData((byte)0x0A, 10L)]
    [InlineData((byte)0x64, 100L)]
    public void Signed8Bit_PositiveValues_DecodeCorrectly(byte rawValue, long expected)
    {
        var db = CreateDbWithSignedSignal(0, 8);
        var data = new byte[8];
        data[0] = rawValue;
        DecodeSignedValue(db, data).Should().Be(expected);
    }
}
