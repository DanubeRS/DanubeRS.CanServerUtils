using System.Collections;
using System.Diagnostics.CodeAnalysis;
using DanubeRS.CanServerUtils.Lib.Parser;
using ValueType = DanubeRS.CanServerUtils.Lib.Parser.ValueType;

namespace DanubeRS.CanServerUtils.Lib;

public static class Decoder
{
    public static SignalValue GetSignalValue(BitArray data, Database.SignalDefinition signal)
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

        long rawValue;
        if (signal.Signal.ValueType == ValueType.Unsigned)
            rawValue = (long)BitConverter.ToUInt64(signalValueBytes);
        else
        {
            var valueAsInt64 = BitConverter.ToInt64(signalValueBytes);
            var shiftAmount = 64 - signal.Signal.Size;
            rawValue = (valueAsInt64 << shiftAmount) >> shiftAmount;
        }

        return new SignalValue(signal.Signal.Name, signal.Signal.Offset + rawValue * signal.Signal.Factor);
    }

    public static void ExtractBitsLittleEndian(BitArray bitArray, int startBit, int signalSize, Span<byte> outputBytes)
    {
        ulong value = 0;

        for (int i = 0; i < signalSize && startBit + i < bitArray.Length; i++)
        {
            if (bitArray[startBit + i])
                value |= (1UL << i);
        }

        for (int i = 0; i < Math.Min((signalSize + 7) / 8, outputBytes.Length); i++)
            outputBytes[i] = (byte)((value >> (i * 8)) & 0xFF);
    }

    public static void ExtractBitsBigEndian(BitArray bitArray, int startBit, int signalSize, Span<byte> outputBytes)
    {
        ulong value = 0;

        for (int i = 0; i < signalSize && startBit + i < bitArray.Length; i++)
        {
            if (bitArray[startBit + i])
                value |= (1UL << (signalSize - 1 - i));
        }

        for (int i = 0; i < Math.Min((signalSize + 7) / 8, outputBytes.Length); i++)
            outputBytes[i] = (byte)((value >> (i * 8)) & 0xFF);
    }

    public static bool TryParseRegularMessage(Database.MessageDefinition defn, BitArray data, [NotNullWhen(true)] out MessageValue? value)
    {
        value = null;
        if (defn.Signals.Count == 0) return true;
        var lastSignal = defn.Signals.OrderByDescending(s => s.Signal.StartBit).First();
        if (lastSignal.Signal.StartBit + lastSignal.Signal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            defn.SignalsByStartBit.Select(signal => GetSignalValue(data, signal)).ToArray(),
            null);
        return true;
    }

    public static bool TryParseMultiplexedMessage(Database.MessageDefinition defn, BitArray data, [NotNullWhen(true)] out MessageValue? value)
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

        var lastSignal = filteredSignals.OrderByDescending(s => s.Signal.StartBit).FirstOrDefault();
        if (lastSignal == null) return false;

        if (lastSignal.Signal.StartBit + lastSignal.Signal.Size > data.Length) return false;

        value = new MessageValue(defn.Header.Id, defn.Header.Name,
            filteredSignals.Select(signal => GetSignalValue(data, signal)).ToArray(), switchValue);

        return true;
    }
}