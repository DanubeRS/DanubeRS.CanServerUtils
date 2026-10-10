using System.Diagnostics.CodeAnalysis;
using DanubeRS.DBCParser.Parser;
using ValueType = DanubeRS.DBCParser.Parser.ValueType;

namespace DanubeRS.DBCParser;

public static class Decoder
{
    private static bool GetBit(ReadOnlySpan<byte> data, int bitIndex)
    {
        return (data[bitIndex >> 3] & (1 << (bitIndex & 7))) != 0;
    }

    public static SignalValue GetSignalValue(ReadOnlySpan<byte> data, Database.SignalDefinition signal)
    {
        Span<byte> signalValueBytes = stackalloc byte[8];

        switch (signal.Signal.Order)
        {
            case Endianness.Little:
                ExtractBitsLittleEndian(data, signal.Signal.StartBit, signal.Signal.Size, signalValueBytes);
                break;
            case Endianness.Big:
                ExtractBitsBigEndian(data, signal.Signal.StartBit, signal.Signal.Size, signalValueBytes);
                break;
        }

        ulong rawValue;
        double value;
        if (signal.Signal.ValueType == ValueType.Unsigned)
        {
            // Unsigned: compute in floating point so the full 64-bit range (max 2^64-1) is
            // preserved — casting through long would fold 2^64-1 into -1.
            rawValue = BitConverter.ToUInt64(signalValueBytes);
            value = signal.Signal.Offset + rawValue * signal.Signal.Factor;
        }
        else
        {
            var valueAsInt64 = BitConverter.ToInt64(signalValueBytes);
            var shiftAmount = 64 - signal.Signal.Size;
            var signedRaw = (valueAsInt64 << shiftAmount) >> shiftAmount;
            value = signal.Signal.Offset + signedRaw * signal.Signal.Factor;
        }

        return new SignalValue(signal.Signal.Name, value);
    }

    public static void ExtractBitsLittleEndian(ReadOnlySpan<byte> data, int startBit, int signalSize, Span<byte> outputBytes)
    {
        ulong value = 0;
        var bitCount = data.Length * 8;

        for (int i = 0; i < signalSize && startBit + i < bitCount; i++)
        {
            if (GetBit(data, startBit + i))
                value |= (1UL << i);
        }

        for (int i = 0; i < Math.Min((signalSize + 7) / 8, outputBytes.Length); i++)
            outputBytes[i] = (byte)((value >> (i * 8)) & 0xFF);
    }

    /// <summary>
    /// Big-endian (Motorola, '@0') extraction per the DBC spec: startBit is the MSB position in
    /// LSB0 numbering; subsequent bits descend toward the LSB of the same byte, then continue at
    /// the MSB of the next byte (sawtooth sequence). Master's previous implementation read an
    /// ascending linear range starting at startBit, which decodes real Motorola frames wrongly
    /// (verified against production Model3CAN UnixTimeSeconds528, 7|32@0 on 65 53 F1 00 → 1 700 000 000).
    /// </summary>
    public static void ExtractBitsBigEndian(ReadOnlySpan<byte> data, int startBit, int signalSize, Span<byte> outputBytes)
    {
        var bitPos = startBit;
        ulong raw = 0;
        for (var i = 0; i < signalSize; i++)
        {
            if (GetBit(data, bitPos))
                raw |= 1UL << (signalSize - 1 - i);

            bitPos = bitPos % 8 == 0
                ? bitPos + 15 // reached the LSB of this byte → jump to the MSB of the next
                : bitPos - 1;
        }

        for (var i = 0; i < Math.Min((signalSize + 7) / 8, outputBytes.Length); i++)
            outputBytes[i] = (byte)((raw >> (i * 8)) & 0xFF);
    }

    public static bool TryParseRegularMessage(Database.MessageDefinition defn, ReadOnlySpan<byte> data, [NotNullWhen(true)] out MessageValue? value)
    {
        value = null;
        if (defn.Signals.Length == 0)
        {
            // Known message without signals: succeed with an empty value rather than
            // returning true with a null value (violating the [NotNullWhen(true)] contract).
            value = new MessageValue(defn.Header.Id, defn.Header.Name, [], null);
            return true;
        }

        foreach (var sd in defn.Signals)
        {
            if (!sd.Bounds.FitsIn(data.Length * 8)) return false;
        }

        var signals = new SignalValue[defn.SignalsByStartBit.Length];
        for (int i = 0; i < defn.SignalsByStartBit.Length; i++)
            signals[i] = GetSignalValue(data, defn.SignalsByStartBit[i]);

        value = new MessageValue(defn.Header.Id, defn.Header.Name, signals, null);
        return true;
    }

    public static bool TryParseMultiplexedMessage(Database.MessageDefinition defn, ReadOnlySpan<byte> data, [NotNullWhen(true)] out MessageValue? value)
    {
        value = null;

        var multiplexSignal = defn.Signals.FirstOrDefault(s => s.Signal.Multiplex is { IsSwitch: true });
        if (multiplexSignal == null)
            return false;

        Span<byte> muxValueBytes = stackalloc byte[8];

        switch (multiplexSignal.Signal.Order)
        {
            case Endianness.Little:
                ExtractBitsLittleEndian(data, multiplexSignal.Signal.StartBit, multiplexSignal.Signal.Size, muxValueBytes);
                break;
            case Endianness.Big:
                ExtractBitsBigEndian(data, multiplexSignal.Signal.StartBit, multiplexSignal.Signal.Size, muxValueBytes);
                break;
        }

        ulong rawSwitchValue;
        if (multiplexSignal.Signal.Size <= 8)
            rawSwitchValue = muxValueBytes[0];
        else if (multiplexSignal.Signal.Size <= 16)
            rawSwitchValue = BitConverter.ToUInt16(muxValueBytes);
        else if (multiplexSignal.Signal.Size <= 32)
            rawSwitchValue = BitConverter.ToUInt32(muxValueBytes);
        else
            rawSwitchValue = BitConverter.ToUInt64(muxValueBytes);

        var switchValue = (uint)rawSwitchValue;

        var filteredSignals = defn.Signals
            .Where(s => s.Signal.Multiplex?.IsSwitch == true
                      || s.Signal.Multiplex == null
                      || s.Signal.Multiplex.SwitchValue == switchValue)
            .ToArray();

        if (filteredSignals.Length == 0)
            return false;

        foreach (var sd in filteredSignals)
        {
            if (!sd.Bounds.FitsIn(data.Length * 8)) return false;
        }

        var signalValues = new SignalValue[filteredSignals.Length];
        for (int i = 0; i < filteredSignals.Length; i++)
            signalValues[i] = GetSignalValue(data, filteredSignals[i]);

        value = new MessageValue(defn.Header.Id, defn.Header.Name, signalValues, switchValue);

        return true;
    }
}