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

namespace DanubeRS.CANServer.Dash.ViewModels;

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
    
    private uint _interfaceRate = 0;
    public uint InterfaceRate
    {
        get => _interfaceRate;
        set => this.RaiseAndSetIfChanged(ref _interfaceRate, value);
    }

    private async Task Listen()
    {
        var dbc = new Database(NullLogger<Database>.Instance);
        foreach (var file in (string[])
                 ["DanubeRS.CANServer.Dash.DBC.CANServer.dbc", "DanubeRS.CANServer.Dash.DBC.Model3CAN.dbc"])
        {
            await using var dbcFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(file);
            if (dbcFile == null) return;
            using var sr = new StreamReader(dbcFile);
            dbc.AddFile(sr);
        }

        var url = "192.168.8.243";
        var clientFactory = new PandasClientFactory(url, 1338, NullLoggerFactory.Instance);
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
                    var battPow = (decimal?)(battCurr?.Value * battVolt?.Value);
                    if (battPow == null) continue;
                    if (battPow != BatteryPower)
                        Dispatcher.UIThread.Post(() => BatteryPower = battPow.Value);
                    break;
                }
                case 0x332:
                {
                    var battTemp = (decimal?)messageValue.Signals.FirstOrDefault(s => s.SignalName == "BattBrickTempMax332")?.Value;
                    if (battTemp == null) continue;
                    if (battTemp != BatteryTemp)
                        Dispatcher.UIThread.Post(() => BatteryTemp = battTemp.Value);
                    break;
                }
                case 0x292:
                {
                    var battSoC = (decimal?)messageValue.Signals.FirstOrDefault(s => s.SignalName == "SOCUI292")?.Value;
                    if (battSoC == null) continue;
                    if (battSoC != BatterySoC)
                        Dispatcher.UIThread.Post(() => BatterySoC = (decimal)battSoC.Value);
                    break;
                }
                case 0x502:
                {
                    var ifRate = (uint?)messageValue.Signals.FirstOrDefault(s => s.SignalName == "CANServer_LoggingRate")?.Value;
                    if (ifRate == null) continue;
                    if (ifRate != InterfaceRate)
                        Dispatcher.UIThread.Post(() => InterfaceRate = ifRate.Value);
                    break;
                }
            }
        }
    }
}
