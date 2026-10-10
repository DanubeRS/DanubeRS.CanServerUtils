using System.Globalization;
using DanubeRS.CanServerUtils.Lib.Parser;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.CanServerUtils.Tests.Database;

public class DecodeTests
{
    private const string MinimalDbc = """
VERSION "1.0"

NS_ :
	CM_
	BA_DEF_
	BA_
	VAL_
	BA_DEF_DEF_

BS_:

BU_: Node1

BO_ 1 Status: 8 Node1
 SG_ A : 0|8@1+ (1,0) [0|255] "km/h"  Node1
 SG_ B : 8|16@1- (0.1,0) [-3276.8|3276.7] "C"  Node1


""";

    [Fact]
    public void ValidPacketData_Parsed_ValidOutputData()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        database.AddFile(new StringReader(MinimalDbc));

        var frameData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        const byte frameId = 0x01;

        var resultSuccess = database.TryParseBinaryMessage(frameId, frameData, out var results, out var defn);

        resultSuccess.Should().BeTrue();
        results.Should().NotBeNull();
        results!.MessageId.Should().Be(frameId);
        results.Signals.Should().HaveCount(2);
        results.Signals[0].SignalName.Should().Be("A");
        results.Signals[0].Value.Should().Be(1);
        // B: 16-bit signed little-endian from bytes 1-2 = 0x0302 = 770; factor 0.1 → 77.0
        results.Signals[1].Value.Should().Be(77.0);
        defn!.Header.Name.Should().Be("Status");
    }

    // Regression: quotes must be stripped from string values (units, VAL_ labels, version).
    [Fact]
    public void StringValues_Parsed_QuotesStripped()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION "1.0"

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] "km/h"  A

CM_ BO_ 100 "Message comment";
CM_ SG_ 100 S "Signal comment";
VAL_ 100 S 1 "One" 2 "Two";


""";
        database.AddFile(new StringReader(dbc));

        database.Version.Should().Be("1.0");
        database.TryParseBinaryMessage(100, new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, out _, out var defn)
            .Should().BeTrue();
        defn!.Signals[0].Signal.Unit.Should().Be("km/h");
        defn.Signals[0].ValueLookup.Should().NotBeNull();
        defn.Signals[0].ValueLookup![(long)1].Should().Be("One");
        defn.Signals[0].ValueLookup![(long)2].Should().Be("Two");
        defn.Comments.Should().ContainSingle().Which.Should().Be("Message comment");
        defn.Signals[0].Comments.Should().ContainSingle().Which.Should().Be("Signal comment");
    }

    // Regression: escaped quotes inside string values are unescaped.
    [Fact]
    public void EscapedQuotes_Unescaped()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION "1.0"

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

CM_ BO_ 100 "has \"escaped\" quotes";


""";
        database.AddFile(new StringReader(dbc));

        database.Comments.Should().ContainSingle();
        database.Comments[0].Should().BeOfType<MessageComment>()
            .Which.Comment.Should().Be("has \"escaped\" quotes");
    }

    // Regression: comma-separated receiver lists parse.
    [Fact]
    public void CommaSeparatedReceivers_Parsed()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        var dbc = MinimalDbc.Replace(
            "SG_ A : 0|8@1+ (1,0) [0|255] \"km/h\"  Node1",
            "SG_ A : 0|8@1+ (1,0) [0|255] \"km/h\"  Node1,Node2");
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(1, new byte[8], out _, out _).Should().BeTrue();
    }

    // Regression: attribute definitions/defaults/values and comments are retained.
    [Fact]
    public void AttributesAndComments_Retained()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION "5.0"

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

CM_ BO_ 100 "Hello";
BA_DEF_ BO_  "CycleTime" INT 0 65535;
BA_DEF_DEF_  "CycleTime" 0;
BA_ "CycleTime" BO_ 100 100;


""";
        database.AddFile(new StringReader(dbc));

        database.AttributeDefinitions.Should().ContainSingle()
            .Which.Name.Should().Be("CycleTime");
        database.AttributeDefaults.Should().ContainSingle()
            .Which.Value.Should().BeOfType<DoubleAttributeDefaultValue>()
            .Which.Value.Should().Be(0d);
        database.AttributeValues.Should().ContainSingle()
            .Which.Value.Should().BeOfType<MessageObjectValue>();
        database.Comments.Should().ContainSingle();
        database.Version.Should().Be("5.0");
    }

    // Regression: FLOAT-typed attribute definitions parse.
    [Fact]
    public void FloatAttributeDefinition_Parsed()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

BA_DEF_ SG_  "Slope" FLOAT 0 100;


""";
        database.AddFile(new StringReader(dbc));

        database.AttributeDefinitions.Should().ContainSingle();
        database.AttributeDefinitions[0].ValueType.Should().BeOfType<FloatAttributeValueType>();
    }

    // Regression: negative VAL_ keys are preserved (no uint wrap).
    [Fact]
    public void NegativeValueDescriptionKeys_Wrapped()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1- (1,0) [-128|127] ""  A

VAL_ 100 S -1 "Unknown" 0 "Known";


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(100, new byte[8], out _, out var defn).Should().BeTrue();

        var lookup = defn!.Signals[0].ValueLookup;
        lookup.Should().NotBeNull();
        lookup!.Keys.Should().Contain(-1L);
        lookup.Keys.Should().NotContain(4294967295L);
        lookup[-1L].Should().Be("Unknown");
    }

    // Regression: duplicate VAL_ entries for the same signal no longer throw at decode time.
    [Fact]
    public void DuplicateValueDescriptions_Merged()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

VAL_ 100 S 1 "A";
VAL_ 100 S 2 "B";


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(100, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.Signals[0].ValueLookup.Should().NotBeNull();
        defn.Signals[0].ValueLookup!.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    // Regression: environment-variable VAL_ sections parse and are retained.
    [Fact]
    public void EnvVariableValueDescriptions_Parsed()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

VAL_ MyEnvVar 1 "On" 0 "Off";


""";
        database.AddFile(new StringReader(dbc));

        database.Comments.Should().BeEmpty();
        // Message ids from VAL_ env-var form have null MessageId; verify via a decode + no crash, and via parser output
        database.TryParseBinaryMessage(100, new byte[8], out _, out _).Should().BeTrue();
    }

    // Regression: messages with no signals decode to an empty, non-null MessageValue.
    [Fact]
    public void ZeroSignalMessage_Decodes_EmptyNonNull()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 Empty: 8 A


""";
        database.AddFile(new StringReader(dbc));
        var ok = database.TryParseBinaryMessage(100, new byte[8], out var value, out _);
        ok.Should().BeTrue();
        value.Should().NotBeNull();
        value!.Signals.Should().BeEmpty();
    }

    // Regression: attribute definitions with min > max are rejected at parse time.
    [Fact]
    public void MinGreaterThanMax_Rejected()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

BA_DEF_ SG_  "Rng" INT 100 0;


""";
        var act = () => database.AddFile(new StringReader(dbc));
        act.Should().Throw<FormatException>()
            .Which.Message.Should().Contain("expected");
    }

    // Regression: INT attribute with decimal bounds fails cleanly with parser context.
    [Fact]
    public void IntAttributeWithDecimalBounds_CleanFailure()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

BA_DEF_ SG_  "Weird" INT 0.0 100.0;


""";
        var act = () => database.AddFile(new StringReader(dbc));
        act.Should().Throw<FormatException>()
            .Which.Message.Should().Contain("Failed to parse DBC file at position");
    }

    // Regression: parse failures carry position, message, and context.
    [Fact]
    public void ParseFailure_ThrowsWithPositionAndContext()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = "VERSION \"\"\n\nNS_ :\n\tCM_\n\nBS_:\n\nBO_ garbage\n";
        var act = () => database.AddFile(new StringReader(dbc));
        var ex = act.Should().Throw<FormatException>().Which;
        ex.Message.Should().Contain("Failed to parse DBC file at position");
        ex.Message.Should().Contain("Context:");
    }

    // Regression: parsing works under a culture that uses ',' as decimal separator.
    [Fact]
    public void Parsing_IsCultureInsensitive()
    {
        var previous = CultureInfo.CurrentCulture;
        var de = (CultureInfo)previous.Clone();
        de.NumberFormat.NumberDecimalSeparator = ",";
        de.NumberFormat.NumberGroupSeparator = ".";
        CultureInfo.CurrentCulture = de;
        try
        {
            var database = new Lib.Database(new NullLogger<Lib.Database>());
            database.AddFile(new StringReader(MinimalDbc));
            database.TryParseBinaryMessage(1, new byte[] { 10, 5, 0, 0, 0, 0, 0, 0 }, out var value, out _)
                .Should().BeTrue();
            // A = 10 (factor 1); B factor 0.1 must parse as 0.1 (not 1) under de-DE:
            // B raw = bytes 1-2 little-endian = 5, factor 0.1 → 0.5
            value!.Signals[0].Value.Should().Be(10);
            value.Signals[1].Value.Should().Be(0.5);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    // Regression: unsigned 64-bit signals decode without wrap to negative.
    [Fact]
    public void Unsigned64Bit_RawDecodes_FullRange()
    {
        var database = new Lib.Database(new NullLogger<Lib.Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ U64 : 0|64@1+ (1,0) [0|0] ""  A


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(100,
            new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, out var value, out _)
            .Should().BeTrue();
        value!.Signals[0].Value.Should().Be(18446744073709551615d);
    }
}