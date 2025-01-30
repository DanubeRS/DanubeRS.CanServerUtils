using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DanubeRS.CANServer.Lib.Pandas;
using DanubeRS.CanServerUtils.Lib;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;

namespace DanubeRS.CANServerDash.ViewModels;

public partial class MainViewModel : ReactiveObject
{
    public MainViewModel()
    {
        Listen().ConfigureAwait(false);
    }
    
    private decimal _batteryPower = 0;
    public decimal BatteryPower
    {
        get => _batteryPower;
        set => this.RaiseAndSetIfChanged(ref _batteryPower, value);
    }
    
    private decimal _batteryTemp = 0;
    public decimal BatteryTemp
    {
        get => _batteryTemp;
        set => this.RaiseAndSetIfChanged(ref _batteryTemp, value);
    }
    
    private decimal _batterySoC = 0;
    public decimal BatterySoC
    {
        get => _batterySoC;
        set => this.RaiseAndSetIfChanged(ref _batterySoC, value);
    }

    private async Task Listen()
    {
        var dbc = new Database(NullLogger<Database>.Instance);
        foreach (var file in (string[])
                 ["DanubeRS.CANServerDash.DBC.CANServer.dbc", "DanubeRS.CANServerDash.DBC.Model3CAN.dbc"])
        {
            await using var dbcFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(file);
            if (dbcFile == null) return;
            using var sr = new StreamReader(dbcFile);
            dbc.AddFile(sr);
        }

        var clientFactory = new PandasClientFactory("192.168.4.1", 1338, NullLoggerFactory.Instance);
        var client = await clientFactory.CreateAsync((message) => HandlePandaMessages(message, dbc));
        await client.Track((0x01, (0x01, 0x32)), (0x01, (0x03, 0x32)), (0x01, (0x02, 0x92)));
        await client.AliveHandle;
    }

    private void HandlePandaMessages(PandasMessage message, Database dbc)
    {
        foreach (var frame in message.Frames)
        {
            if (!dbc.TryParseBinaryMessage(frame.FrameId, frame.FrameData, out var messageValue,
                    out var messageDefinition)) continue;
            switch (frame.FrameId)
            {
                case 0x132:
                {
                    var battCurr = messageValue.Signals.FirstOrDefault(s => s.SignalName == "SmoothBattCurrent132");
                    var battVolt = messageValue.Signals.FirstOrDefault(s => s.SignalName == "BattVoltage132");
                    var battPow = battCurr?.Value * battVolt?.Value;
                    if (battPow == null) continue;
                    Dispatcher.UIThread.Post(() => BatteryPower = (decimal)battPow);
                    break;
                }
                case 0x332:
                {
                    var battTemp = messageValue.Signals.FirstOrDefault(s => s.SignalName == "BattBrickTempMax332");
                    if (battTemp == null) continue;
                    Dispatcher.UIThread.Post(() => BatteryTemp = (decimal)battTemp.Value);
                    break;
                }
                case 0x292:
                {
                    var battSoC = messageValue.Signals.FirstOrDefault(s => s.SignalName == "SOCUI292");
                    if (battSoC == null) continue;
                    Dispatcher.UIThread.Post(() => BatterySoC = (decimal)battSoC.Value);
                    break;
                }
            }
        }
    }
}
