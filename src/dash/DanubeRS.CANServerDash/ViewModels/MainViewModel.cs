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
    
    private decimal _Slider = 0;
    public decimal Slider
    {
        get => _Slider;
        set
        {
            this.RaiseAndSetIfChanged(ref _Slider, value);
        }
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

        var clientFactory = new PandasClientFactory("192.168.8.243", 1338, NullLoggerFactory.Instance);
        var client = await clientFactory.CreateAsync((message) => HandlePandaMessages(message, dbc));
        await client.Track((0x01, [0x02, 0x43]));
        await client.AliveHandle;
    }

    private void HandlePandaMessages(PandasMessage message, Database dbc)
    {
        foreach (var frame in message.Frames)
        {
            if (!dbc.TryParseBinaryMessage(frame.FrameId, frame.FrameData, out var messageValue,
                    out var messageDefinition)) continue;
            if (frame.FrameId != 0x243) continue;
            foreach (var signal in messageDefinition.Signals.Where(s => s.Signal.Name == "VCRIGHT_hvacCabinTempEst"))
            {
                var value = messageValue.Signals.Single(s => s.SignalName == signal.Signal.Name).Value;
                Dispatcher.UIThread.Post(() => Slider = (decimal)value);
            }
        }
    }
}
