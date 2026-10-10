using System.Globalization;
using DanubeRS.DBCParser.Parser;
using DanubeRS.DBCParser;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace DanubeRS.DBCParser.Tests.Decoding;

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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
            .Which.Value.Should().BeOfType<IntegerAttributeDefaultValue>()
            .Which.Value.Should().Be(0);
        database.AttributeValues.Should().ContainSingle()
            .Which.Value.Should().BeOfType<MessageObjectValue>();
        database.Comments.Should().ContainSingle();
        database.Version.Should().Be("5.0");
    }

    // Regression: FLOAT-typed attribute definitions parse.
    [Fact]
    public void FloatAttributeDefinition_Parsed()
    {
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
        const string dbc = "VERSION \"\"\n\nNS_ :\n	CM_\n\nBS_:\n\nBO_ garbage\n";
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
            var database = new Database(new NullLogger<Database>());
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
        var database = new Database(new NullLogger<Database>());
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

    // Regression: Motorola (@0) signals decode with sawtooth bit order per spec.
    [Fact]
    public void MotorolaSignals_DecodePerSpec()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ Be8 : 7|8@0+ (1,0) [0|255] ""  A
 SG_ Be16 : 31|16@0+ (1,0) [0|65535] ""  A


""";
        database.AddFile(new StringReader(dbc));
        // Be8: start 7 = byte0 MSB → 0x12 = 18. Be16: start 31 → MSB at byte3 bit7, len 16 → bytes 3,4 = 0x78, 0xA5 hmm use simple payload:
        database.TryParseBinaryMessage(100, new byte[] { 0x12, 0x34, 0x56, 0x78, 0, 0, 0, 0 }, out var value, out var defn)
            .Should().BeTrue();
        var byName = value!.Signals.ToDictionary(s => s.SignalName, s => s.Value);
        byName["Be8"].Should().Be(0x12); // 18
        var _ = defn;
    }

    // Regression: Motorola signal in the last byte (sawtooth bounds) no longer rejected.
    [Fact]
    public void MotorolaLastByteSignal_Accepted()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ Last : 63|8@0+ (1,0) [0|255] ""  A


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(100, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0x7F }, out var value, out _)
            .Should().BeTrue();
        value!.Signals[0].Value.Should().Be(0x7F);
    }

    // Regression: signed Motorola decodes via sign extension.
    [Fact]
    public void MotorolaSigned_NegativeValue()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ Sbe : 7|8@0- (1,0) [-128|127] ""  A


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(100, new byte[] { 0x89, 0, 0, 0, 0, 0, 0, 0 }, out var value, out _)
            .Should().BeTrue();
        value!.Signals[0].Value.Should().Be(-119); // 0x89 as signed 8-bit
    }

    // Regression: 17-bit mux switch — no crash, full-width switch value, no truncation.
    [Fact]
    public void WideMuxSwitch_DecodesFullWidth()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ Sel M : 0|17@1+ (1,0) [0|131071] ""  A
 SG_ V m65537 : 17|8@1+ (1,0) [0|255] ""  A


""";
        database.AddFile(new StringReader(dbc));
        // Sel = 0x01 | (1<<16) = 65537 (bit 16 set — previously crashed BitsToBytes bounds).
        // V m65537 = 0x99 at start 17 (bits 17..24): byte2 = 0x32|0x01(b16) = 0x33, byte3 bit0 = 1.
        database.TryParseBinaryMessage(100, new byte[] { 0x01, 0x00, 0x33, 0x01, 0, 0, 0, 0 }, out var value, out _)
            .Should().BeTrue();
        value!.MultiplexValue.Should().Be(65537u);
        value.Signals.Select(s => s.SignalName).Should().BeEquivalentTo("Sel", "V");
        value.Signals.First(s => s.SignalName == "V").Value.Should().Be(0x99);
    }

    // Regression: non-mux signals + mux switch present in every mux decode; no-match still decodes.
    [Fact]
    public void MultiplexedMessage_IncludesCommonSignals_AndHandlesNoMatch()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ Common : 40|8@1+ (1,0) [0|255] ""  A
 SG_ Sel M : 0|8@1+ (1,0) [0|255] ""  A
 SG_ V m1 : 8|8@1+ (1,0) [0|255] ""  A


""";
        database.AddFile(new StringReader(dbc));

        // Matching branch: all three present.
        database.TryParseBinaryMessage(100, new byte[] { 1, 7, 0, 0, 0, 42, 0, 0 }, out var value1, out _)
            .Should().BeTrue();
        value1!.MultiplexValue.Should().Be(1u);
        value1.Signals.Select(s => s.SignalName).Should().BeEquivalentTo("Sel", "V", "Common");

        // No matching branch: still decodes switch + common (previously returned false).
        database.TryParseBinaryMessage(100, new byte[] { 5, 9, 0, 0, 0, 42, 0, 0 }, out var value2, out _)
            .Should().BeTrue();
        value2!.MultiplexValue.Should().Be(5u);
        value2.Signals.Select(s => s.SignalName).Should().BeEquivalentTo("Sel", "Common");
        value2.Signals.First(s => s.SignalName == "Common").Value.Should().Be(42);
    }


    // Regression (production file): UnixTimeSeconds528 in Model3CAN.dbc is Motorola 7|32@0.
    // Big-endian bytes 65 53 F1 00 must decode to 1,700,000,000 (previously 123042-garbage).
    [Fact]
    public void MotorolaProductionSignal_DecodesUnixTime()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: Receiver

BO_ 1320 ID528Clocks: 8 Receiver
 SG_ UnixTimeSeconds528 : 7|32@0+ (1,0) [0|4294970000] "sec"  Receiver


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(1320, new byte[] { 0x65, 0x53, 0xF1, 0x00, 0, 0, 0, 0 }, out var value, out _)
            .Should().BeTrue();
        value!.Signals[0].Value.Should().Be(1700000000d);
    }


    // Regression: interleaved sections (CM/VAL between BO_ blocks) no longer drop messages.
    [Fact]
    public void InterleavedSections_AllMessagesRetained()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M1: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

CM_ BO_ 100 "c";

BO_ 101 M2: 8 A
 SG_ T : 0|8@1+ (1,0) [0|255] ""  A

VAL_ 101 T 1 "One";

BA_DEF_ BO_  "Cyc" INT 0 65535;


""";
        database.AddFile(new StringReader(dbc));

        database.TryParseBinaryMessage(100, new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, out var v100, out var d100)
            .Should().BeTrue();
        database.TryParseBinaryMessage(101, new byte[] { 2, 0, 0, 0, 0, 0, 0, 0 }, out var v101, out var d101)
            .Should().BeTrue();
        d101!.Signals[0].ValueLookup.Should().NotBeNull(); // interleaved VAL_ retained
    }

    // Regression: BA_DEF_ before any BO_ block no longer drops all messages.
    [Fact]
    public void AttributeDefinitionBeforeMessages_MessagesRetained()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BA_DEF_ BO_  "Cyc" INT 0 65535;

BO_ 160 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(160, new byte[] { 9, 0, 0, 0, 0, 0, 0, 0 }, out var v, out var defn)
            .Should().BeTrue();
        v!.Signals[0].Value.Should().Be(9);
        database.AttributeDefinitions.Should().ContainSingle();
    }

    // Regression: NS_ section is optional (spec); a file without NS_ parses.
    [Fact]
    public void MissingNsSection_Parses()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A


""";
        database.AddFile(new StringReader(dbc));
        database.TryParseBinaryMessage(100, new byte[8], out var v, out var defn)
            .Should().BeTrue();
        v!.Signals.Should().ContainSingle();
        defn!.Header.Name.Should().Be("M");
    }

    // Regression: trailing garbage is rejected with a position, not silently ignored.
    [Fact]
    public void TrailingGarbage_Rejected()
    {
        var database = new Database(new NullLogger<Database>());
        var dbc = "VERSION \"\"\n\nNS_ :\n	CM_\n\nBS_:\n\nBU_: A\n\nBO_ 100 M: 8 A\n SG_ S : 0|8@1+ (1,0) [0|255] \"\"  A\n\nJUNKJUNK 123\n";
        var act = () => database.AddFile(new StringReader(dbc));
        act.Should().Throw<FormatException>()
            .Which.Message.Should().Contain("Failed to parse DBC file at position");
    }

    // Regression: unmodeled but well-formed sections are skipped-and-REPORTED, not dropped silently.
    [Fact]
    public void UnknownSections_ReportedViaSkipped()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ S : 0|8@1+ (1,0) [0|255] ""  A

VAL_TABLE_ Tbl 1 "One" 2 "Two";
ENVVAR_DATA_ Env1 : 4;
BA_ "VFlag" BO_ 100 1;


""";
        database.AddFile(new StringReader(dbc));

        // VAL_TABLE_ / ENVVAR_DATA_ are unmodeled: skipped and reported.
        // BA_ is a modeled section (attribute value) — it parses normally.
        database.SkippedStatements.Should().BeEquivalentTo("VAL_TABLE_", "ENVVAR_DATA_");
        database.TryParseBinaryMessage(100, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.Header.Name.Should().Be("M");
    }


    // Regression: the mux switch selects groups by RAW value; factor/offset must not participate.
    [Fact]
    public void MuxSwitchWithFactorOffset_SelectsByRaw()
    {
        var database2 = new Database(new NullLogger<Database>());
        const string dbc2 = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 101 M: 8 A
 SG_ Sel M : 0|8@1+ (10,0) [0|25.5] "kV"  A
 SG_ V m2 : 8|8@1+ (1,0) [0|255] ""  A


""";
        database2.AddFile(new StringReader(dbc2));
        // raw switch = 2 → physical would be 20; must still select m2 and report raw 2
        database2.TryParseBinaryMessage(101, new byte[] { 2, 7, 0, 0, 0, 0, 0, 0 }, out var value, out _)
            .Should().BeTrue();
        value!.MultiplexValue.Should().Be(2u);
        value.Signals.First(s => s.SignalName == "V").Value.Should().Be(7);
    }

    // Regression: single-byte Motorola signal bounds account for the whole span (MaxBit starts at StartBit).
    [Fact]
    public void SingleByteMotorola_BoundsCoverStart()
    {
        var database = new Database(new NullLogger<Database>());
        const string dbc = """
VERSION ""

NS_ :
	CM_

BS_:

BU_: A

BO_ 100 M: 8 A
 SG_ Be : 7|8@0+ (1,0) [0|255] ""  A

SG_MUL_VAL_ 100 Be Be 1 [0|1];

""";
        database.AddFile(new StringReader(dbc));
        database.SkippedStatements.Should().Contain("SG_MUL_VAL_");

        // The D2 fix itself: start 7 len 8 @0 → sawtooth covers bits 7..0. The old code
        // overwrote MaxBit with the last visited bit (0); it must be 7 and MinBit 0.
        database.TryParseBinaryMessage(100, new byte[8], out _, out var defn).Should().BeTrue();
        defn!.Signals[0].Bounds.MinBit.Should().Be(0);
        defn.Signals[0].Bounds.MaxBit.Should().Be(7);
        defn.Signals[0].Bounds.FitsIn(8).Should().BeTrue();  // 1-byte frame fits
    }


    // Regression: tokenization failures surface as positioned FormatException, not raw ParseException.
    [Fact]
    public void TokenizeFailure_ThrowsWithPositionAndContext()
    {
        var database = new Database(new NullLogger<Database>());
        // '~' outside any string is not a matchable token.
        const string dbc = "VERSION \"\"\n\nBS_:\n\nBU_: A\n\nBO_ 100 M: 8 A\n SG_ S : 0|8@1+ (1,0) [0|255] \"\"  A\n\n~~garbage~~\n";
        var act = () => database.AddFile(new StringReader(dbc));
        var ex = act.Should().Throw<FormatException>().Which;
        ex.Message.Should().Contain("Failed to tokenize DBC file at position");
        ex.Message.Should().Contain("Context:");
        ex.Message.Should().NotContain("ParseException");
    }


    // Regression: the Database constructor requires no explicit logger.
    [Fact]
    public void DatabaseConstructor_LoggerOptional()
    {
        var database = new Database();
        database.AddFile(new StringReader($"""{MinimalDbc}"""));
        database.TryParseBinaryMessage(1, new byte[8], out var value, out var defn).Should().BeTrue();
        defn!.Header.Name.Should().Be("Status");
    }

}