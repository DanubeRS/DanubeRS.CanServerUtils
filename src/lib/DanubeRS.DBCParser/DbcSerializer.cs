using System.Globalization;
using System.Text;
using DanubeRS.DBCParser.Parser;
using DbcValueType = DanubeRS.DBCParser.Parser.ValueType;

namespace DanubeRS.DBCParser;

public static class DbcSerializer
{
    public static string Serialize(ParsedDb db)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"VERSION \"{db.Version.VersionString}\"");
        sb.AppendLine();
        sb.AppendLine("NS_ : ");
        foreach (var ns in db.NewSymbols.NamespaceValues)
            sb.AppendLine($"\t{ns}");
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("BS_:");
        sb.AppendLine();
        sb.AppendLine();
        sb.Append($"BU_: {string.Join(" ", db.Nodes.NodeList)}");
        sb.AppendLine();
        sb.AppendLine();

        foreach (var message in db.Messages.OrderBy(m => m.Header.Id))
        {
            sb.AppendLine();
            sb.AppendLine($"BO_ {message.Header.Id} {message.Header.Name}: {message.Header.Size} {message.Header.Node}");
            foreach (var signal in message.Signals.OrderBy(s => s.StartBit))
            {
                var multiplex = signal.Multiplex switch
                {
                    { IsSwitch: true } => " M",
                    { SwitchValue: not null } => $" m{signal.Multiplex.SwitchValue}",
                    _ => ""
                };
                var valueTypeIndicator = signal.ValueType == DbcValueType.Signed ? "-" : "+";
                var endiannessIndicator = signal.Order == Endianness.Little ? "1" : "0";
                sb.AppendLine($" SG_ {signal.Name}{multiplex} : {signal.StartBit}|{signal.Size}@{endiannessIndicator}{valueTypeIndicator} ({FormatDouble(signal.Factor)},{FormatDouble(signal.Offset)}) [{FormatDouble(signal.Min)}|{FormatDouble(signal.Max)}] \"{signal.Unit}\" {signal.Receiver}");
            }
            sb.AppendLine();
        }

        if ((db.AttributeDefinitions?.Length ?? 0) > 0)
        {
            sb.AppendLine();
            foreach (var attrDef in db.AttributeDefinitions ?? [])
            {
                var objectType = attrDef.ObjectType switch
                {
                    "BO_" => "BO_ ",
                    "SG_" => "SG_ ",
                    "BU_" => "BU_ ",
                    "EV_" => "EV_ ",
                    _ => ""
                };
                sb.AppendLine($"BA_DEF_ {objectType}\"{attrDef.Name}\" {FormatAttributeValueType(attrDef.ValueType)}");
            }
        }

        if ((db.AttributeDefaults?.Length ?? 0) > 0)
        {
            sb.AppendLine();
            foreach (var attrDefault in db.AttributeDefaults ?? [])
            {
                sb.AppendLine($"BA_DEF_DEF_ \"{attrDefault.AttributeName}\" {FormatAttributeDefaultValue(attrDefault.Value)};");
            }
        }

        if ((db.AttributeValues?.Length ?? 0) > 0)
        {
            sb.AppendLine();
            foreach (var attrValue in db.AttributeValues ?? [])
            {
                var objectRef = attrValue.Value switch
                {
                    GlobalObjectValue => "",
                    NodeObjectValue nov => $" BU_ {nov.Name}",
                    MessageObjectValue mov => $" BO_ {mov.MessageId}",
                    SignalObjectValue sov => $" SG_ {sov.MessageId} {sov.SignalName}",
                    EnvironmentObjectValue eov => $" EV_ {eov.Name}",
                    _ => ""
                };
                sb.AppendLine($"BA_ \"{attrValue.Name}\"{objectRef} {FormatAttributeDefaultValue(attrValue.Value.Value)};");
            }
        }

        if ((db.ValueDescriptions?.Length ?? 0) > 0)
        {
            sb.AppendLine();
            foreach (var vd in db.ValueDescriptions ?? [])
            {
                var values = string.Join(" ", vd.Values.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key} \"{kv.Value}\""));
                sb.AppendLine($"VAL_ {vd.MessageId} {vd.SignalName} {values};");
            }
        }

        sb.AppendLine();
        return sb.ToString();
    }

    private static string FormatDouble(double value)
    {
        if (value == Math.Floor(value) && Math.Abs(value) < 1e15)
            return ((long)value).ToString(CultureInfo.InvariantCulture);
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static string FormatAttributeValueType(AttributeValueType type)
    {
        return type switch
        {
            IntAttributeValueType i => $"INT {i.Min} {i.Max};",
            HexAttributeValueType h => $"HEX {h.Min} {h.Max};",
            FloatAttributeValueType f => $"FLOAT {FormatDouble(f.Min)} {FormatDouble(f.Max)};",
            StringAttributeValueType => "STRING ;",
            EnumAttributeValueType e => $"{string.Join(", ", e.Values)};",
            _ => ";"
        };
    }

    private static string FormatAttributeDefaultValue(AttributeDefaultValue value)
    {
        return value switch
        {
            IntegerAttributeDefaultValue i => i.Value.ToString(CultureInfo.InvariantCulture),
            DoubleAttributeDefaultValue d => d.Value.ToString(CultureInfo.InvariantCulture),
            StringAttributeDefaultValue s => $"\"{s.Value}\"",
            _ => ""
        };
    }
}