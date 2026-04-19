using DanubeRS.CanServerUtils.Lib;
using DanubeRS.CanServerUtils.Lib.Parser;
using FluentAssertions;
using DbcValueType = DanubeRS.CanServerUtils.Lib.Parser.ValueType;
using DbcVersion = DanubeRS.CanServerUtils.Lib.Parser.Version;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class DbcValidatorTests
{
    private static Signal CreateSignal(
        string name = "TestSignal",
        int startBit = 0,
        int size = 8,
        Endianness order = Endianness.Little,
        DbcValueType valueType = DbcValueType.Unsigned,
        double factor = 1.0,
        double offset = 0.0,
        double min = 0.0,
        double max = 255.0,
        string unit = "",
        string receiver = "Vector",
        Multiplex? multiplex = null) =>
        new(name, multiplex, startBit, size, order, valueType, factor, offset, min, max, unit, receiver);

    private static Message CreateMessage(uint id = 1, string name = "TestMsg", int size = 8,
        string node = "Vector", Signal[]? signals = null) =>
        new(new MessageHeader(id, name, size, node), signals ?? [CreateSignal()]);

    private static ParsedDb CreateParsedDb(Message[]? messages = null) =>
        new(new DbcVersion(""),
            new NewSymbols([]),
            new Nodes([]),
            messages ?? [CreateMessage()],
            [],
            [],
            [],
            []);

    [Fact]
    public void ValidSignal_PassesValidation()
    {
        var signal = CreateSignal(name: "Speed", startBit: 0, size: 8, factor: 0.01);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8, originalSignalName: "Speed");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void OverlappingSignals_Detected()
    {
        var signal1 = CreateSignal(name: "Speed", startBit: 0, size: 8);
        var signal2 = CreateSignal(name: "Temp", startBit: 4, size: 8);
        var message = CreateMessage(signals: [signal1, signal2]);

        var result = DbcValidator.ValidateSignalEdit(signal2, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("overlaps") && e.Contains("Speed"));
    }

    [Fact]
    public void NonOverlappingSignals_PassValidation()
    {
        var signal1 = CreateSignal(name: "Speed", startBit: 0, size: 8);
        var signal2 = CreateSignal(name: "Temp", startBit: 8, size: 8);
        var message = CreateMessage(signals: [signal1, signal2]);

        var result = DbcValidator.ValidateSignalEdit(signal2, message, 8, originalSignalName: "Temp");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void MultiplexedSignals_WithDifferentSwitchValues_AllowOverlap()
    {
        var muxSwitch = CreateSignal(name: "Mux", startBit: 0, size: 4,
            multiplex: new Multiplex(true, null));
        var signal1 = CreateSignal(name: "OptionA", startBit: 8, size: 8,
            multiplex: new Multiplex(false, 1));
        var signal2 = CreateSignal(name: "OptionB", startBit: 8, size: 8,
            multiplex: new Multiplex(false, 2));
        var message = CreateMessage(signals: [muxSwitch, signal1, signal2]);

        var result = DbcValidator.ValidateSignalEdit(signal2, message, 8, originalSignalName: "OptionB");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StartBitOutOfRange_Rejected()
    {
        var signal = CreateSignal(name: "Bad", startBit: 100, size: 8);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("exceeds message size"));
    }

    [Fact]
    public void StartBitNegative_Rejected()
    {
        var signal = CreateSignal(name: "Bad", startBit: -1, size: 8);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("StartBit must be >= 0"));
    }

    [Fact]
    public void SignalExceedingMessageBoundary_Rejected()
    {
        var signal = CreateSignal(name: "Bad", startBit: 56, size: 16);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("exceeds message size"));
    }

    [Fact]
    public void SizeZero_Rejected()
    {
        var signal = CreateSignal(name: "Bad", size: 0);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Size must be >= 1"));
    }

    [Fact]
    public void SizeExceeds64_Rejected()
    {
        var signal = CreateSignal(name: "Bad", size: 65);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Size must be <= 64"));
    }

    [Fact]
    public void ZeroFactor_Rejected()
    {
        var signal = CreateSignal(name: "Bad", factor: 0.0);
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Factor must not be zero"));
    }

    [Fact]
    public void DuplicateSignalName_Rejected()
    {
        var signal1 = CreateSignal(name: "Speed", startBit: 0, size: 8);
        var signal2 = CreateSignal(name: "Speed", startBit: 8, size: 8);
        var message = CreateMessage(signals: [signal1, signal2]);

        var result = DbcValidator.ValidateSignalEdit(signal2, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already exists"));
    }

    [Fact]
    public void EmptySignalName_Rejected()
    {
        var signal = CreateSignal(name: "");
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("must not be empty"));
    }

    [Fact]
    public void InvalidSignalNameChars_Rejected()
    {
        var signal = CreateSignal(name: "Bad Signal!");
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("invalid characters"));
    }

    [Fact]
    public void MuxSwitchWithSwitchValue_Rejected()
    {
        var signal = CreateSignal(name: "Bad", startBit: 0, size: 4,
            multiplex: new Multiplex(true, 1));
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("must not have a SwitchValue"));
    }

    [Fact]
    public void MuxSignalWithoutSwitchValue_Rejected()
    {
        var signal = CreateSignal(name: "Bad", startBit: 8, size: 8,
            multiplex: new Multiplex(false, null));
        var message = CreateMessage(signals: [signal]);
        var result = DbcValidator.ValidateSignalEdit(signal, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("must have a SwitchValue"));
    }

    [Fact]
    public void DuplicateMuxSwitch_Rejected()
    {
        var switch1 = CreateSignal(name: "Mux1", startBit: 0, size: 4,
            multiplex: new Multiplex(true, null));
        var switch2 = CreateSignal(name: "Mux2", startBit: 4, size: 4,
            multiplex: new Multiplex(true, null));
        var message = CreateMessage(signals: [switch1, switch2]);

        var result = DbcValidator.ValidateSignalEdit(switch2, message, 8);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already has a multiplexer switch"));
    }

    [Fact]
    public void GetOccupiedBits_ReturnsCorrectBitPositions()
    {
        var signal = CreateSignal(startBit: 3, size: 5);
        var bits = DbcValidator.GetOccupiedBits(signal);
        bits.Should().BeEquivalentTo([3, 4, 5, 6, 7]);
    }

    [Fact]
    public void GetOccupiedBits_SingleBit()
    {
        var signal = CreateSignal(startBit: 0, size: 1);
        var bits = DbcValidator.GetOccupiedBits(signal);
        bits.Should().BeEquivalentTo([0]);
    }

    [Fact]
    public void SignalsOverlap_NonOverlapping_ReturnsFalse()
    {
        var a = CreateSignal(startBit: 0, size: 8);
        var b = CreateSignal(startBit: 8, size: 8);
        DbcValidator.SignalsOverlap(a, b).Should().BeFalse();
    }

    [Fact]
    public void SignalsOverlap_Overlapping_ReturnsTrue()
    {
        var a = CreateSignal(startBit: 0, size: 8);
        var b = CreateSignal(startBit: 4, size: 8);
        DbcValidator.SignalsOverlap(a, b).Should().BeTrue();
    }

    [Fact]
    public void ValidateNewMessage_DuplicateFrameId_Rejected()
    {
        var existing = CreateMessage(id: 0x100, name: "Msg1");
        var db = CreateParsedDb(messages: [existing]);
        var newMsg = CreateMessage(id: 0x100, name: "Msg2");

        var result = DbcValidator.ValidateNewMessage(newMsg.Header, db);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("0x100") && e.Contains("Msg1"));
    }

    [Fact]
    public void ValidateNewMessage_DuplicateName_Rejected()
    {
        var existing = CreateMessage(id: 0x100, name: "SameName");
        var db = CreateParsedDb(messages: [existing]);
        var newMsg = CreateMessage(id: 0x200, name: "SameName");

        var result = DbcValidator.ValidateNewMessage(newMsg.Header, db);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("SameName"));
    }

    [Fact]
    public void ValidateNewMessage_InvalidSize_Rejected()
    {
        var db = CreateParsedDb(messages: []);
        var header = new MessageHeader(1, "Msg", 0, "Node");

        var result = DbcValidator.ValidateNewMessage(header, db);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("1 and 8"));
    }

    [Fact]
    public void ValidateMessageEdit_AllSignalsValid_Passes()
    {
        var signal1 = CreateSignal(name: "A", startBit: 0, size: 8);
        var signal2 = CreateSignal(name: "B", startBit: 8, size: 8);
        var message = CreateMessage(signals: [signal1, signal2]);
        var db = CreateParsedDb(messages: [message]);

        var result = DbcValidator.ValidateMessageEdit(message, db);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateNewSignal_SameAsSignalEdit()
    {
        var existing = CreateSignal(name: "A", startBit: 0, size: 8);
        var message = CreateMessage(signals: [existing]);
        var newSignal = CreateSignal(name: "B", startBit: 8, size: 8);

        var result = DbcValidator.ValidateNewSignal(newSignal, message, 8);
        result.IsValid.Should().BeTrue();
    }
}