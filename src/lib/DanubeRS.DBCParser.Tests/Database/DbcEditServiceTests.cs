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

    private static async Task<DbcEditService> CreateAndLoadAsync()
    {
        var service = CreateService();
        await service.LoadDbcAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestDbc)));
        return service;
    }

    [Fact]
    public async Task LoadDbcAsync_ValidContent_IsLoadedTrue()
    {
        var service = await CreateAndLoadAsync();
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
    public async Task GetMessagesAsync_ReturnsAllMessages()
    {
        var service = await CreateAndLoadAsync();
        var messages = await service.GetMessagesAsync();
        messages.Should().HaveCount(1);
        messages[0].Header.Name.Should().Be("TestMsg");
    }

    [Fact]
    public async Task GetMessageAsync_ExistingId_ReturnsMessage()
    {
        var service = await CreateAndLoadAsync();
        var msg = await service.GetMessageAsync(1);
        msg.Should().NotBeNull();
        msg!.Header.Name.Should().Be("TestMsg");
    }

    [Fact]
    public async Task GetMessageAsync_NonexistentId_ReturnsNull()
    {
        var service = await CreateAndLoadAsync();
        (await service.GetMessageAsync(999)).Should().BeNull();
    }

    [Fact]
    public async Task GetMessageDefinitionAsync_ReturnsDefinitionForFrame()
    {
        var service = await CreateAndLoadAsync();
        var defn = await service.GetMessageDefinitionAsync(1);
        defn.Should().NotBeNull();
        defn!.Header.Id.Should().Be(1u);
        defn.Signals.Count.Should().Be(4);
    }

    [Fact]
    public async Task UpdateSignalAsync_ChangesSignalProperties()
    {
        var service = await CreateAndLoadAsync();
        var original = (await service.GetMessageAsync(1))!.Signals.First(s => s.Name == "Unsigned8");
        var updated = original with { StartBit = 48, Size = 8 };

        await service.UpdateSignalAsync(1, "Unsigned8", updated);

        var msg = (await service.GetMessageAsync(1))!;
        var changed = msg.Signals.First(s => s.Name == "Unsigned8");
        changed.StartBit.Should().Be(48);
        changed.Size.Should().Be(8);
    }

    [Fact]
    public async Task UpdateSignalAsync_OverlappingBits_ThrowsValidationException()
    {
        var service = await CreateAndLoadAsync();
        var original = (await service.GetMessageAsync(1))!.Signals.First(s => s.Name == "Unsigned8");
        var overlapping = original with { StartBit = 8 };

        var act = () => service.UpdateSignalAsync(1, "Unsigned8", overlapping);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateSignalAsync_NonexistentFrame_ThrowsKeyNotFoundException()
    {
        var service = await CreateAndLoadAsync();
        var act = () => service.UpdateSignalAsync(999, "Unsigned8", new Signal("X", null, 0, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector"));
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddSignalAsync_NewValidSignal_AddsSuccessfully()
    {
        var service = await CreateAndLoadAsync();
        var newSignal = new Signal("NewSignal", null, 48, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector");

        await service.AddSignalAsync(1, newSignal);

        var msg = (await service.GetMessageAsync(1))!;
        msg.Signals.Should().Contain(s => s.Name == "NewSignal");
    }

    [Fact]
    public async Task AddSignalAsync_OverlappingName_ThrowsValidationException()
    {
        var service = await CreateAndLoadAsync();
        var duplicate = new Signal("Unsigned8", null, 48, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector");

        var act = () => service.AddSignalAsync(1, duplicate);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RemoveSignalAsync_ExistingSignal_RemovesSuccessfully()
    {
        var service = await CreateAndLoadAsync();
        await service.RemoveSignalAsync(1, "Unsigned8");

        var msg = (await service.GetMessageAsync(1))!;
        msg.Signals.Should().NotContain(s => s.Name == "Unsigned8");
    }

    [Fact]
    public async Task RemoveSignalAsync_NonexistentSignal_ThrowsKeyNotFoundException()
    {
        var service = await CreateAndLoadAsync();
        var act = () => service.RemoveSignalAsync(1, "Nonexistent");
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AddMessageAsync_NewValidMessage_AddsSuccessfully()
    {
        var service = await CreateAndLoadAsync();
        var header = new MessageHeader(2, "NewMsg", 8, "Vector");
        await service.AddMessageAsync(header);

        (await service.GetMessagesAsync()).Should().Contain(m => m.Header.Id == 2);
    }

    [Fact]
    public async Task AddMessageAsync_DuplicateFrameId_ThrowsValidationException()
    {
        var service = await CreateAndLoadAsync();
        var header = new MessageHeader(1, "NewMsg", 8, "Vector");
        var act = () => service.AddMessageAsync(header);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RemoveMessageAsync_ExistingId_RemovesSuccessfully()
    {
        var service = await CreateAndLoadAsync();
        await service.RemoveMessageAsync(1);
        (await service.GetMessagesAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveMessageAsync_NonexistentId_ThrowsKeyNotFoundException()
    {
        var service = await CreateAndLoadAsync();
        var act = () => service.RemoveMessageAsync(999);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ExportDbcAsync_ProducesValidDbcThatReParses()
    {
        var service = await CreateAndLoadAsync();
        var exported = await service.ExportDbcAsync();
        exported.Should().NotBeNullOrEmpty();

        var service2 = CreateService();
        await service2.LoadDbcAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(exported)));
        (await service2.GetMessagesAsync()).Should().HaveCount(1);
    }

    [Fact]
    public async Task ExportDbcAsync_ReflectsSignalEdits()
    {
        var service = await CreateAndLoadAsync();
        var original = (await service.GetMessageAsync(1))!.Signals.First(s => s.Name == "Unsigned8");
        await service.UpdateSignalAsync(1, "Unsigned8", original with { StartBit = 48 });

        var exported = await service.ExportDbcAsync();
        var service2 = CreateService();
        await service2.LoadDbcAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(exported)));

        var reparsed = (await service2.GetMessageAsync(1))!;
        var edited = reparsed.Signals.First(s => s.Name == "Unsigned8");
        edited.StartBit.Should().Be(48);
    }

    [Fact]
    public async Task BuildDefinitionWithOverridesAsync_ReturnsDefinitionWithOverrides()
    {
        var service = await CreateAndLoadAsync();
        var overrides = new Dictionary<string, Signal>
        {
            ["Unsigned8"] = new Signal("Unsigned8", null, 48, 8, Endianness.Little, DbcValueType.Unsigned, 1, 0, 0, 255, "", "Vector")
        };

        var defn = await service.BuildDefinitionWithOverridesAsync(1, overrides);
        defn.Should().NotBeNull();
        var changed = defn.Signals.First(s => s.Signal.Name == "Unsigned8");
        changed.Signal.StartBit.Should().Be(48);
    }

    [Fact]
    public async Task BuildDefinitionWithOverridesAsync_DoesNotMutateOriginal()
    {
        var service = await CreateAndLoadAsync();
        var originalMsg = (await service.GetMessageAsync(1))!;
        var originalSignal = originalMsg.Signals.First(s => s.Name == "Unsigned8");

        var overrides = new Dictionary<string, Signal>
        {
            ["Unsigned8"] = originalSignal with { StartBit = 48 }
        };

        await service.BuildDefinitionWithOverridesAsync(1, overrides);

        var originalAfter = (await service.GetMessageAsync(1))!;
        var originalStill = originalAfter.Signals.First(s => s.Name == "Unsigned8");
        originalStill.StartBit.Should().Be(originalSignal.StartBit);
    }

    [Fact]
    public async Task LoadDbcAsync_SecondLoad_ReplacesPrevious()
    {
        var service = await CreateAndLoadAsync();
        (await service.GetMessagesAsync()).Should().HaveCount(1);

        const string differentDbc = """
            VERSION ""

            NS_ :

            BS_:

            BU_: ECU

            BO_ 100 StatusMsg: 8 ECU
             SG_ Speed : 0|8@1+ (0.1,0) [0|25.5] "km/h" ECU


            """;
        await service.LoadDbcAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(differentDbc)));
        (await service.GetMessagesAsync()).Should().HaveCount(1);
        (await service.GetMessageAsync(100))!.Header.Name.Should().Be("StatusMsg");
    }
}