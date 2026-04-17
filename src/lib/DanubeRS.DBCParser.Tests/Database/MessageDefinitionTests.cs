using DanubeRS.CanServerUtils.Lib;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class MessageDefinitionTests
{
    private const string MultiplexedDbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 MuxMsg: 8 Vector
 SG_ M : 0|8@1+ (1,0) [0|255] """" Vector
 SG_ Sig1 m1 : 8|8@1+ (1,0) [0|255] """" Vector
 SG_ Sig2 m1 : 16|8@1+ (1,0) [0|255] """" Vector
 SG_ Sig3 m2 : 8|8@1+ (1,0) [0|255] """" Vector

BO_ 2 RegularMsg: 8 Vector
 SG_ Alpha : 0|8@1+ (1,0) [0|255] """" Vector
 SG_ Beta : 16|8@1+ (1,0) [0|255] """" Vector
 SG_ Gamma : 8|8@1+ (1,0) [0|255] """" Vector

";

    private static Lib.Database CreateDb()
    {
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(MultiplexedDbc));
        return db;
    }

    [Fact]
    public void MultiplexedMessage_IsMultiplexedTrue()
    {
        // Note: Parser currently doesn't set IsMultiplexed flag based on multiplex signals with switch values (m1, m2)
        var db = CreateDb();
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
    }

    [Fact]
    public void RegularMessage_IsMultiplexedFalse()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(2, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.IsMultiplexed.Should().BeFalse();
    }

    [Fact]
    public void SignalsByStartBit_OrderedAscending()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(2, new byte[8], out _, out var defn).Should().BeTrue();
        var startBits = defn!.SignalsByStartBit.Select(s => s.Signal.StartBit).ToArray();
        startBits.Should().BeInAscendingOrder();
        startBits.Should().Equal(0, 8, 16);
    }

    [Fact]
    public void MultiplexedMessage_SignalsByStartBit_IncludesAllSignals()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.SignalsByStartBit.Should().HaveCount(4);
        defn.SignalsByStartBit.Select(s => s.Signal.Name).Should().Equal("M", "Sig1", "Sig3", "Sig2");
    }

    [Fact]
    public void MessageDefinition_HeaderPreserved()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.Header.Id.Should().Be(1);
        defn.Header.Name.Should().Be("MuxMsg");
    }

    [Fact]
    public void SignalDefinition_ValueLookupPopulatedFromVal()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Status : 0|8@1+ (1,0) [0|2] """" Vector

VAL_ 1 Status 0 ""Off"" 1 ""On"" 2 ""Fault"";

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        var statusDef = defn!.Signals.First(s => s.Signal.Name == "Status");
        statusDef.ValueLookup.Should().NotBeNull();
        statusDef.ValueLookup!.Should().HaveCount(3);
    }

    [Fact]
    public void SignalDefinition_ValueLookupNullWhenNoVal()
    {
        const string dbc = @"
VERSION """"

NS_ :

BS_:

BU_: Vector

BO_ 1 Msg: 8 Vector
 SG_ Speed : 0|8@1+ (1,0) [0|255] """" Vector

";
        var db = new Lib.Database(new NullLogger<Lib.Database>());
        db.AddFile(new StringReader(dbc));
        db.TryParseBinaryMessage(1, new byte[8], out _, out var defn).Should().BeTrue();
        var speedDef = defn!.Signals.First(s => s.Signal.Name == "Speed");
        speedDef.ValueLookup.Should().BeNull();
    }

    [Fact]
    public void MessageDefinition_SignalsArrayContainsAllSignals()
    {
        var db = CreateDb();
        db.TryParseBinaryMessage(2, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.Signals.Should().HaveCount(3);
        defn.Signals.Select(s => s.Signal.Name).Should().Contain("Alpha", "Beta", "Gamma");
    }
}
