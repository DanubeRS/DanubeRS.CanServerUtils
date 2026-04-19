using DanubeRS.CanServerUtils.Lib;
using DanubeRS.CanServerUtils.Lib.Parser;
using DanubeRS.DBCParser;
using FluentAssertions;
using LibDatabase = DanubeRS.CanServerUtils.Lib.Database;
using DbcValueType = DanubeRS.CanServerUtils.Lib.Parser.ValueType;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class DbcEditServiceTests
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

    private static DbcEditService CreateService()
    {
        return new DbcEditService(new Microsoft.Extensions.Logging.Abstractions.NullLogger<DbcEditService>());
    }

    private static DbcEditService CreateAndLoad()
    {
        var service = CreateService();
        service.LoadDbc(TestDbc);
        return service;
    }

    [Fact]
    public void LoadDbc_ValidContent_IsLoadedTrue()
    {
        var service = CreateAndLoad();
        service.IsLoaded.Should().BeTrue();
    }

    [Fact]
    public void LoadDbc_InvalidContent_ThrowsException()
    {
        var service = CreateService();
        var act = () => service.LoadDbc("INVALID DBC CONTENT!!!");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void GetMessages_ReturnsAllMessages()
    {
        var service = CreateAndLoad();
        var messages = service.GetMessages();
        messages.Should().HaveCount(1);
        messages[0].Header.Name.Should().Be("TestMsg");
    }

    [Fact]
    public void GetMessage_ExistingId_ReturnsMessage()
    {
        var service = CreateAndLoad();
        var msg = service.GetMessage(1);
        msg.Should().NotBeNull();
        msg!.Header.Name.Should().Be("TestMsg");
    }

    [Fact]
    public void GetMessage_NonexistentId_ReturnsNull()
    {
        var service = CreateAndLoad();
        service.GetMessage(999).Should().BeNull();
    }

    [Fact]
    public void GetMessageDefinition_ReturnsDefinitionForFrame()
    {
        var service = CreateAndLoad();
        var defn = service.GetMessageDefinition(1);
        defn.Should().NotBeNull();
        defn!.Header.Id.Should().Be(1u);
        defn.Signals.Count.Should().Be(4);
    }

    [Fact]
    public void UpdateSignal_ChangesSignalProperties()
    {
        var service = CreateAndLoad();
        var original = service.GetMessage(1)!.Signals.First(s => s.Name == "Unsigned8");
        var updated = original with { StartBit = 48, Size = 8 };

        service.UpdateSignal(1, "Unsigned8", updated);

        var msg = service.GetMessage(1)!;
        var changed = msg.Signals.First(s => s.Name == "Unsigned8");
        changed.StartBit.Should().Be(48);
        changed.Size.Should().Be(8);
    }

    [Fact]
    public void UpdateSignal_OverlappingBits_ThrowsValidationException()
    {
        var service = CreateAndLoad();
        var original = service.GetMessage(1)!.Signals.First(s => s.Name == "Unsigned8");
        var overlapping = original with { StartBit = 8 };

        var act = () => service.UpdateSignal(1, "Unsigned8", overlapping);
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void UpdateSignal_NonexistentFrame_ThrowsKeyNotFoundException()
    {
        var service = CreateAndLoad();
        var act = () => service.UpdateSignal(999, "Unsigned8", new Signal("X", null, 0, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector"));
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void AddSignal_NewValidSignal_AddsSuccessfully()
    {
        var service = CreateAndLoad();
        var newSignal = new Signal("NewSignal", null, 48, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector");

        service.AddSignal(1, newSignal);

        var msg = service.GetMessage(1)!;
        msg.Signals.Should().Contain(s => s.Name == "NewSignal");
    }

    [Fact]
    public void AddSignal_OverlappingName_ThrowsValidationException()
    {
        var service = CreateAndLoad();
        var duplicate = new Signal("Unsigned8", null, 48, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector");

        var act = () => service.AddSignal(1, duplicate);
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void RemoveSignal_ExistingSignal_RemovesSuccessfully()
    {
        var service = CreateAndLoad();
        service.RemoveSignal(1, "Unsigned8");

        var msg = service.GetMessage(1)!;
        msg.Signals.Should().NotContain(s => s.Name == "Unsigned8");
    }

    [Fact]
    public void RemoveSignal_NonexistentSignal_ThrowsKeyNotFoundException()
    {
        var service = CreateAndLoad();
        var act = () => service.RemoveSignal(1, "Nonexistent");
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void AddMessage_NewValidMessage_AddsSuccessfully()
    {
        var service = CreateAndLoad();
        var header = new MessageHeader(2, "NewMsg", 8, "Vector");
        service.AddMessage(header);

        service.GetMessages().Should().Contain(m => m.Header.Id == 2);
    }

    [Fact]
    public void AddMessage_DuplicateFrameId_ThrowsValidationException()
    {
        var service = CreateAndLoad();
        var header = new MessageHeader(1, "NewMsg", 8, "Vector");
        var act = () => service.AddMessage(header);
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void RemoveMessage_ExistingId_RemovesSuccessfully()
    {
        var service = CreateAndLoad();
        service.RemoveMessage(1);
        service.GetMessages().Should().BeEmpty();
    }

    [Fact]
    public void RemoveMessage_NonexistentId_ThrowsKeyNotFoundException()
    {
        var service = CreateAndLoad();
        var act = () => service.RemoveMessage(999);
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void ExportDbc_ProducesValidDbcThatReParses()
    {
        var service = CreateAndLoad();
        var exported = service.ExportDbc();
        exported.Should().NotBeNullOrEmpty();

        var service2 = CreateService();
        service2.LoadDbc(exported);
        service2.GetMessages().Should().HaveCount(1);
    }

    [Fact]
    public void ExportDbc_ReflectsSignalEdits()
    {
        var service = CreateAndLoad();
        var original = service.GetMessage(1)!.Signals.First(s => s.Name == "Unsigned8");
        service.UpdateSignal(1, "Unsigned8", original with { StartBit = 48 });

        var exported = service.ExportDbc();
        var service2 = CreateService();
        service2.LoadDbc(exported);

        var reparsed = service2.GetMessage(1)!;
        var edited = reparsed.Signals.First(s => s.Name == "Unsigned8");
        edited.StartBit.Should().Be(48);
    }

    [Fact]
    public void BuildDefinitionWithOverrides_ReturnsDefinitionWithOverrides()
    {
        var service = CreateAndLoad();
        var overrides = new Dictionary<string, Signal>
        {
            ["Unsigned8"] = new Signal("Unsigned8", null, 48, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector")
        };

        var defn = service.BuildDefinitionWithOverrides(1, overrides);
        defn.Should().NotBeNull();
        var changed = defn.Signals.First(s => s.Signal.Name == "Unsigned8");
        changed.Signal.StartBit.Should().Be(48);
    }

    [Fact]
    public void BuildDefinitionWithOverrides_DoesNotMutateOriginal()
    {
        var service = CreateAndLoad();
        var originalMsg = service.GetMessage(1)!;
        var originalSignal = originalMsg.Signals.First(s => s.Name == "Unsigned8");

        var overrides = new Dictionary<string, Signal>
        {
            ["Unsigned8"] = originalSignal with { StartBit = 48 }
        };

        service.BuildDefinitionWithOverrides(1, overrides);

        var originalAfter = service.GetMessage(1)!;
        var originalStill = originalAfter.Signals.First(s => s.Name == "Unsigned8");
        originalStill.StartBit.Should().Be(originalSignal.StartBit);
    }

    [Fact]
    public void LoadDbc_SecondLoad_ReplacesPrevious()
    {
        var service = CreateAndLoad();
        service.GetMessages().Should().HaveCount(1);

        const string differentDbc = """
            VERSION ""

            NS_ :

            BS_:

            BU_: ECU

            BO_ 100 StatusMsg: 8 ECU
             SG_ Speed : 0|8@1+ (0.1,0) [0|25.5] "km/h" ECU


            """;
        service.LoadDbc(differentDbc);
        service.GetMessages().Should().HaveCount(1);
        service.GetMessage(100)!.Header.Name.Should().Be("StatusMsg");
    }
}