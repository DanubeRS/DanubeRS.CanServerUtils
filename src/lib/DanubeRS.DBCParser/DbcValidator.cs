using System.Linq;
using DanubeRS.CanServerUtils.Lib.Parser;

namespace DanubeRS.CanServerUtils.Lib;

public class ValidationResult
{
    public List<string> Errors { get; } = [];
    public List<string> Warnings { get; } = [];
    public bool IsValid => Errors.Count == 0;

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);

    public ValidationResult Merge(ValidationResult other)
    {
        Errors.AddRange(other.Errors);
        Warnings.AddRange(other.Warnings);
        return this;
    }
}

public static class DbcValidator
{
    private static readonly System.Text.RegularExpressions.Regex SignalNameRegex = new(@"^[A-Za-z_][A-Za-z0-9_]*$", System.Text.RegularExpressions.RegexOptions.Compiled);

    public static ValidationResult ValidateSignalEdit(Signal newSignal, Message targetMessage, int messageSizeBytes, string? originalSignalName = null)
    {
        var result = new ValidationResult();
        ValidateSignalProperties(newSignal, messageSizeBytes, result);
        ValidateSignalOverlap(newSignal, targetMessage, result);
        ValidateDuplicateSignalName(newSignal, targetMessage, result, originalSignalName);
        ValidateMultiplexSwitchCount(newSignal, targetMessage, result);
        return result;
    }

    public static ValidationResult ValidateNewSignal(Signal newSignal, Message targetMessage, int messageSizeBytes)
    {
        return ValidateSignalEdit(newSignal, targetMessage, messageSizeBytes);
    }

    public static ValidationResult ValidateMessageEdit(Message newMessage, ParsedDb existingDb)
    {
        var result = new ValidationResult();
        ValidateMessageProperties(newMessage, result);
        ValidateDuplicateFrameId(newMessage, existingDb, result);
        ValidateDuplicateMessageName(newMessage, existingDb, result);

        foreach (var signal in newMessage.Signals)
        {
            ValidateSignalProperties(signal, newMessage.Header.Size, result);
            ValidateSignalOverlapWithContext(signal, newMessage.Signals, newMessage, result);
            ValidateDuplicateSignalNameForNewSignal(signal, newMessage.Signals, result);
            ValidateMultiplexSwitchCount(signal, newMessage, result);
        }

        return result;
    }

    public static ValidationResult ValidateNewMessage(MessageHeader header, ParsedDb existingDb)
    {
        var result = new ValidationResult();

        if (header.Size < 1 || header.Size > 64)
            result.AddError($"Message size must be between 1 and 64 bytes, got {header.Size}");

        if (string.IsNullOrWhiteSpace(header.Name))
            result.AddError("Message name must not be empty");

        var existingById = existingDb.Messages.FirstOrDefault(m => m.Header.Id == header.Id);
        if (existingById != null)
            result.AddError($"Frame ID 0x{header.Id:X} already used by message '{existingById.Header.Name}'");

        var existingByName = existingDb.Messages.FirstOrDefault(m => m.Header.Name == header.Name);
        if (existingByName != null)
            result.AddError($"Message name '{header.Name}' already used by message 0x{existingByName.Header.Id:X}");

        return result;
    }

    public static HashSet<int> GetOccupiedBits(Signal signal)
    {
        var bits = new HashSet<int>();
        for (int i = 0; i < signal.Size; i++)
            bits.Add(signal.StartBit + i);
        return bits;
    }

    public static bool SignalsOverlap(Signal a, Signal b)
    {
        var bitsA = GetOccupiedBits(a);
        var bitsB = GetOccupiedBits(b);
        bitsA.IntersectWith(bitsB);
        return bitsA.Count > 0;
    }

    private static void ValidateSignalProperties(Signal signal, int messageSizeBytes, ValidationResult result)
    {
        var maxBit = messageSizeBytes * 8;

        if (signal.StartBit < 0)
            result.AddError($"Signal '{signal.Name}': StartBit must be >= 0, got {signal.StartBit}");

        if (signal.StartBit >= maxBit)
            result.AddError($"Signal '{signal.Name}': StartBit {signal.StartBit} exceeds message size ({messageSizeBytes} bytes = {maxBit} bits)");

        if (signal.Size < 1)
            result.AddError($"Signal '{signal.Name}': Size must be >= 1, got {signal.Size}");

        if (signal.Size > 64)
            result.AddError($"Signal '{signal.Name}': Size must be <= 64, got {signal.Size}");

        if (signal.StartBit + signal.Size > maxBit)
            result.AddError($"Signal '{signal.Name}': StartBit ({signal.StartBit}) + Size ({signal.Size}) = {signal.StartBit + signal.Size} exceeds message size ({maxBit} bits)");

        if (string.IsNullOrWhiteSpace(signal.Name))
            result.AddError("Signal name must not be empty");
        else if (!SignalNameRegex.IsMatch(signal.Name))
            result.AddError($"Signal name '{signal.Name}' contains invalid characters. Only letters, digits, and underscores are allowed, and the name must start with a letter or underscore");

        if (signal.Factor == 0)
            result.AddError($"Signal '{signal.Name}': Factor must not be zero");

        if (!Enum.IsDefined(signal.ValueType))
            result.AddError($"Signal '{signal.Name}': Invalid ValueType");

        if (!Enum.IsDefined(signal.Order))
            result.AddError($"Signal '{signal.Name}': Invalid Endianness");

        if (signal.Multiplex != null)
        {
            if (signal.Multiplex.IsSwitch && signal.Multiplex.SwitchValue != null)
                result.AddError($"Signal '{signal.Name}': Multiplexer switch signal must not have a SwitchValue");
            if (!signal.Multiplex.IsSwitch && signal.Multiplex.SwitchValue == null)
                result.AddError($"Signal '{signal.Name}': Multiplexed signal must have a SwitchValue");
        }
    }

    private static void ValidateSignalOverlap(Signal newSignal, Message targetMessage, ValidationResult result)
    {
        foreach (var existing in targetMessage.Signals)
        {
            if (existing.Name == newSignal.Name)
                continue;

            if (!SignalsOverlap(newSignal, existing))
                continue;

            if (AreMultiplexedWithDifferentSwitchValues(newSignal, existing))
                continue;

            var overlapBits = GetOccupiedBits(newSignal);
            overlapBits.IntersectWith(GetOccupiedBits(existing));
            result.AddError($"Signal '{newSignal.Name}' overlaps with signal '{existing.Name}' at bit positions [{string.Join(", ", overlapBits.Order())}]");
        }
    }

    private static void ValidateSignalOverlapWithContext(Signal newSignal, Signal[] allSignals, Message targetMessage, ValidationResult result)
    {
        foreach (var existing in allSignals)
        {
            if (existing.Name == newSignal.Name)
                continue;

            if (!SignalsOverlap(newSignal, existing))
                continue;

            if (AreMultiplexedWithDifferentSwitchValues(newSignal, existing))
                continue;

            var overlapBits = GetOccupiedBits(newSignal);
            overlapBits.IntersectWith(GetOccupiedBits(existing));
            result.AddError($"Signal '{newSignal.Name}' overlaps with signal '{existing.Name}' at bit positions [{string.Join(", ", overlapBits.Order())}]");
        }
    }

    private static void ValidateDuplicateSignalName(Signal newSignal, Message targetMessage, ValidationResult result, string? excludedName = null)
    {
        var existingWithSameName = targetMessage.Signals.FirstOrDefault(s => s.Name == newSignal.Name && s.Name != excludedName);
        if (existingWithSameName != null)
            result.AddError($"Signal name '{newSignal.Name}' already exists in message '{targetMessage.Header.Name}' (0x{targetMessage.Header.Id:X})");
    }

    private static void ValidateDuplicateSignalNameForNewSignal(Signal signal, Signal[] allSignals, ValidationResult result)
    {
        var count = allSignals.Count(s => s.Name == signal.Name);
        if (count > 1)
            result.AddError($"Duplicate signal name '{signal.Name}' in message. Each signal name must be unique within a message.");
    }

    private static void ValidateMultiplexSwitchCount(Signal newSignal, Message targetMessage, ValidationResult result)
    {
        if (newSignal.Multiplex?.IsSwitch == true)
        {
            var existingSwitches = targetMessage.Signals.Count(s => s.Multiplex?.IsSwitch == true && s.Name != newSignal.Name);
            if (existingSwitches > 0)
                result.AddError($"Message '{targetMessage.Header.Name}' already has a multiplexer switch signal. Only one is allowed per message.");
        }
    }

    private static void ValidateMessageProperties(Message message, ValidationResult result)
    {
        if (message.Header.Size < 1 || message.Header.Size > 64)
            result.AddError($"Message size must be between 1 and 64 bytes, got {message.Header.Size}");

        if (string.IsNullOrWhiteSpace(message.Header.Name))
            result.AddError("Message name must not be empty");
    }

    private static void ValidateDuplicateFrameId(Message newMessage, ParsedDb existingDb, ValidationResult result)
    {
        var existing = existingDb.Messages.FirstOrDefault(m => m.Header.Id == newMessage.Header.Id);
        if (existing != null && existing.Header.Name != newMessage.Header.Name)
            result.AddError($"Frame ID 0x{newMessage.Header.Id:X} already used by message '{existing.Header.Name}'");
    }

    private static void ValidateDuplicateMessageName(Message newMessage, ParsedDb existingDb, ValidationResult result)
    {
        var existing = existingDb.Messages.FirstOrDefault(m => m.Header.Name == newMessage.Header.Name);
        if (existing != null && existing.Header.Id != newMessage.Header.Id)
            result.AddError($"Message name '{newMessage.Header.Name}' already used by message 0x{existing.Header.Id:X}");
    }

    private static bool AreMultiplexedWithDifferentSwitchValues(Signal a, Signal b)
    {
        if (a.Multiplex == null || b.Multiplex == null)
            return false;

        if (a.Multiplex.IsSwitch || b.Multiplex.IsSwitch)
            return false;

        if (a.Multiplex.SwitchValue != null && b.Multiplex.SwitchValue != null)
            return a.Multiplex.SwitchValue != b.Multiplex.SwitchValue;

        return false;
    }
}