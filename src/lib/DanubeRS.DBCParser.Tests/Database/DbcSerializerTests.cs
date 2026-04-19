using DanubeRS.CanServerUtils.Lib;
using DanubeRS.CanServerUtils.Lib.Parser;
using DanubeRS.CanServerUtils.Lib.Tokenizer;
using FluentAssertions;
using LibDatabase = DanubeRS.CanServerUtils.Lib.Database;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class DbcSerializerTests
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

    [Fact]
    public void RoundTrip_BasicDbc_ProducesEquivalentAst()
    {
        var original = Parse(TestDbc);
        var serialized = DbcSerializer.Serialize(original);
        var reparsed = Parse(serialized);

        reparsed.Version.VersionString.Should().Be(original.Version.VersionString);
        reparsed.Nodes.NodeList.Should().BeEquivalentTo(original.Nodes.NodeList);
        reparsed.Messages.Length.Should().Be(original.Messages.Length);

        for (int i = 0; i < original.Messages.Length; i++)
        {
            var origMsg = original.Messages[i];
            var reparsedMsg = reparsed.Messages[i];
            reparsedMsg.Header.Id.Should().Be(origMsg.Header.Id);
            reparsedMsg.Header.Name.Should().Be(origMsg.Header.Name);
            reparsedMsg.Header.Size.Should().Be(origMsg.Header.Size);
            reparsedMsg.Header.Node.Should().Be(origMsg.Header.Node);
            reparsedMsg.Signals.Length.Should().Be(origMsg.Signals.Length);

            for (int j = 0; j < origMsg.Signals.Length; j++)
            {
                var origSig = origMsg.Signals[j];
                var reparsedSig = reparsedMsg.Signals[j];
                reparsedSig.Name.Should().Be(origSig.Name);
                reparsedSig.StartBit.Should().Be(origSig.StartBit);
                reparsedSig.Size.Should().Be(origSig.Size);
                reparsedSig.Order.Should().Be(origSig.Order);
                reparsedSig.ValueType.Should().Be(origSig.ValueType);
                reparsedSig.Factor.Should().Be(origSig.Factor);
                reparsedSig.Offset.Should().Be(origSig.Offset);
                reparsedSig.Min.Should().Be(origSig.Min);
                reparsedSig.Max.Should().Be(origSig.Max);
                reparsedSig.Unit.Should().Be(origSig.Unit);
                reparsedSig.Receiver.Should().Be(origSig.Receiver);
            }
        }
    }

    [Fact]
    public void RoundTrip_BigEndianSignal_PreservesEndianness()
    {
        const string dbc = """
            VERSION ""

            NS_ :

            BS_:

            BU_: ECU

            BO_ 100 StatusMsg: 8 ECU
             SG_ BigEndianSig : 7|16@0+ (0.1,0) [0|6553.5] "rpm" ECU


            """;
        var original = Parse(dbc);
        var serialized = DbcSerializer.Serialize(original);
        var reparsed = Parse(serialized);

        var origSignal = original.Messages[0].Signals[0];
        var reparsedSignal = reparsed.Messages[0].Signals[0];
        reparsedSignal.Order.Should().Be(Endianness.Big);
        reparsedSignal.Order.Should().Be(origSignal.Order);
        reparsedSignal.StartBit.Should().Be(origSignal.StartBit);
        reparsedSignal.Size.Should().Be(origSignal.Size);
    }

    [Fact]
    public void RoundTrip_MultiplexedSignal_PreservesMultiplex()
    {
        const string dbc = """
            VERSION ""

            NS_ :

            BS_:

            BU_: Vector

            BO_ 1 MuxMsg: 8 Vector
             SG_ M M : 0|8@1+ (1,0) [0|255] "" Vector
             SG_ SigA_m1 m1 : 8|16@1+ (1,0) [0|65535] "" Vector
             SG_ SigB_m1 m1 : 24|8@1+ (1,0) [0|255] "" Vector
             SG_ SigA_m2 m2 : 8|16@1+ (1,0) [0|65535] "" Vector


            """;
        var original = Parse(dbc);
        var serialized = DbcSerializer.Serialize(original);
        var reparsed = Parse(serialized);

        var muxSwitch = reparsed.Messages[0].Signals.FirstOrDefault(s => s.Multiplex?.IsSwitch == true);
        muxSwitch.Should().NotBeNull();
        muxSwitch!.Multiplex!.IsSwitch.Should().BeTrue();

        var sigA_m1 = reparsed.Messages[0].Signals.First(s => s.Name == "SigA_m1");
        sigA_m1.Multiplex.Should().NotBeNull();
        sigA_m1.Multiplex!.SwitchValue.Should().Be(1);

        var sigA_m2 = reparsed.Messages[0].Signals.First(s => s.Name == "SigA_m2");
        sigA_m2.Multiplex.Should().NotBeNull();
        sigA_m2.Multiplex!.SwitchValue.Should().Be(2);
    }

    [Fact]
    public void RoundTrip_ValueDescriptions_PreservesValues()
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
        var original = Parse(dbc);
        var serialized = DbcSerializer.Serialize(original);
        var reparsed = Parse(serialized);

        reparsed.ValueDescriptions.Length.Should().Be(1);
        reparsed.ValueDescriptions[0].MessageId.Should().Be(1u);
        reparsed.ValueDescriptions[0].SignalName.Should().Be("Status");
        reparsed.ValueDescriptions[0].Values[0].Should().Be("Off");
        reparsed.ValueDescriptions[0].Values[1].Should().Be("On");
        reparsed.ValueDescriptions[0].Values[2].Should().Be("Fault");
    }

    [Fact]
    public void Serialize_ProducesValidDbcThatDecodesCorrectly()
    {
        var original = Parse(TestDbc);
        var serialized = DbcSerializer.Serialize(original);

        var db = new LibDatabase(new Microsoft.Extensions.Logging.Abstractions.NullLogger<LibDatabase>());
        db.AddFile(new StringReader(serialized));

        var data = new byte[8];
        data[0] = 0xAB;
        db.TryParseBinaryMessage(1, data, out var value, out _).Should().BeTrue();
        value!.Signals.First(s => s.SignalName == "Unsigned8").Value.Should().Be(0xAB);
    }

    private static ParsedDb Parse(string dbcContent)
    {
        var tokens = DbcTokenizer.Instance.Tokenize(dbcContent);
        var result = DanubeRS.CanServerUtils.Lib.Parser.DbcParser.Database(tokens);
        if (!result.HasValue)
            throw new Exception($"Parse failed: {result.ErrorMessage ?? string.Join(", ", result.Expectations ?? [])}");
        return result.Value;
    }
}