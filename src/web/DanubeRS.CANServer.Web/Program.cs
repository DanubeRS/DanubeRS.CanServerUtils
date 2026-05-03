using System.Collections;
using System.Text.Json.Serialization;
using DanubeRS.CANServer.Lib;
using DanubeRS.CANServer.Lib.BinaryLogs;
using DanubeRS.CANServer.Lib.Connections;
using DanubeRS.CanServerUtils.Lib;
using DanubeRS.DBCParser;
using DanubeRS.CANServer.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5238");

builder.Services.AddSingleton<DbcEditService>();
builder.Services.AddSingleton<CanLogService>();
builder.Services.AddSingleton<PandasConnectionService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();

app.MapPost("/api/dbc/load", async (HttpContext context, DbcEditService editService) =>
{
    if (context.Request.ContentLength > 50 * 1024 * 1024)
        return Results.BadRequest(new { error = "DBC file exceeds 50 MB limit" });

    var form = await context.Request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    if (file == null)
        return Results.BadRequest(new { error = "No file uploaded" });

    using var stream = file.OpenReadStream();
    try
    {
        await editService.LoadDbcAsync(stream);
        var messages = await editService.GetMessagesAsync();
        return Results.Ok(new
        {
            messageCount = messages.Count,
            signalCount = messages.Sum(m => m.Signals.Length)
        });
    }
    catch (DbcParseException ex)
    {
        return Results.BadRequest(new { error = "DBC parse error", details = string.Join("; ", ex.Errors) });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/dbc/messages", async (DbcEditService editService) =>
{
    if (!editService.IsLoaded)
        return Results.BadRequest(new { error = "No DBC file loaded" });

    var messages = await editService.GetMessagesAsync();
    return Results.Ok(messages.Select(m => new
    {
        frameId = m.Header.Id,
        name = m.Header.Name,
        size = m.Header.Size,
        node = m.Header.Node,
        signals = m.Signals.Select(s => new
        {
            name = s.Name,
            startBit = s.StartBit,
            size = s.Size,
            order = s.Order.ToString(),
            valueType = s.ValueType.ToString(),
            factor = s.Factor,
            offset = s.Offset,
            min = s.Min,
            max = s.Max,
            unit = s.Unit,
            receiver = s.Receiver,
            multiplex = s.Multiplex == null ? null : new
            {
                isSwitch = s.Multiplex.IsSwitch,
                switchValue = s.Multiplex.SwitchValue
            }
        })
    }));
});

app.MapPut("/api/dbc/messages/{frameId}/signals/{signalName}", async (uint frameId, string signalName, SignalUpdate update, DbcEditService editService) =>
{
    try
    {
        var message = await editService.GetMessageAsync(frameId);
        if (message == null)
            return Results.NotFound(new { error = $"Message with frame ID 0x{frameId:X} not found" });

        var existing = message.Signals.FirstOrDefault(s => s.Name == signalName);
        if (existing == null)
            return Results.NotFound(new { error = $"Signal '{signalName}' not found in message 0x{frameId:X}" });

        var updated = existing with
        {
            Name = update.Name ?? existing.Name,
            StartBit = update.StartBit ?? existing.StartBit,
            Size = update.Size ?? existing.Size,
            Order = update.Order ?? existing.Order,
            ValueType = update.ValueType ?? existing.ValueType,
            Factor = update.Factor ?? existing.Factor,
            Offset = update.Offset ?? existing.Offset,
            Min = update.Min ?? existing.Min,
            Max = update.Max ?? existing.Max,
            Unit = update.Unit ?? existing.Unit,
            Receiver = update.Receiver ?? existing.Receiver,
            Multiplex = update.ClearMultiplex ? null : (update.Multiplex ?? existing.Multiplex)
        };

        var result = await editService.UpdateSignalAsync(frameId, signalName, updated);
        return Results.Ok(new
        {
            name = result.Name,
            startBit = result.StartBit,
            size = result.Size,
            order = result.Order.ToString(),
            valueType = result.ValueType.ToString(),
            factor = result.Factor,
            offset = result.Offset,
            min = result.Min,
            max = result.Max,
            unit = result.Unit,
            receiver = result.Receiver,
            multiplex = result.Multiplex == null ? null : new
            {
                isSwitch = result.Multiplex.IsSwitch,
                switchValue = result.Multiplex.SwitchValue
            }
        });
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { error = "Validation failed", details = string.Join("; ", ex.Errors) });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPost("/api/dbc/messages/{frameId}/signals", async (uint frameId, SignalCreate create, DbcEditService editService) =>
{
    try
    {
        var signal = new DanubeRS.CanServerUtils.Lib.Parser.Signal(
            create.Name,
            create.Multiplex,
            create.StartBit,
            create.Size,
            create.Order ?? DanubeRS.CanServerUtils.Lib.Parser.Endianness.Little,
            create.ValueType ?? DanubeRS.CanServerUtils.Lib.Parser.ValueType.Unsigned,
            create.Factor ?? 1.0,
            create.Offset ?? 0.0,
            create.Min ?? 0.0,
            create.Max ?? 1.0,
            create.Unit ?? "",
            create.Receiver ?? "Vector"
        );

        var result = await editService.AddSignalAsync(frameId, signal);
        return Results.Ok(new
        {
            name = result.Name,
            startBit = result.StartBit,
            size = result.Size
        });
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { error = "Validation failed", details = string.Join("; ", ex.Errors) });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapDelete("/api/dbc/messages/{frameId}/signals/{signalName}", async (uint frameId, string signalName, DbcEditService editService) =>
{
    try
    {
        await editService.RemoveSignalAsync(frameId, signalName);
        return Results.NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPost("/api/dbc/messages", async (MessageCreate create, DbcEditService editService) =>
{
    try
    {
        var header = new DanubeRS.CanServerUtils.Lib.Parser.MessageHeader(create.FrameId, create.Name, create.Size, create.Node ?? "Vector");
        var message = await editService.AddMessageAsync(header);
        return Results.Ok(new
        {
            frameId = message.Header.Id,
            name = message.Header.Name,
            size = message.Header.Size,
            node = message.Header.Node
        });
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { error = "Validation failed", details = string.Join("; ", ex.Errors) });
    }
});

app.MapDelete("/api/dbc/messages/{frameId}", async (uint frameId, DbcEditService editService) =>
{
    try
    {
        await editService.RemoveMessageAsync(frameId);
        return Results.NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapGet("/api/dbc/export", async (DbcEditService editService) =>
{
    if (!editService.IsLoaded)
        return Results.BadRequest(new { error = "No DBC file loaded" });

    try
    {
        var content = await editService.ExportDbcAsync();
        return Results.File(
            System.Text.Encoding.UTF8.GetBytes(content),
            "text/plain",
            "export.dbc"
        );
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/log/upload", async (HttpContext context, CanLogService logService) =>
{
    if (context.Request.ContentLength > 50 * 1024 * 1024)
        return Results.BadRequest(new { error = "Log file exceeds 50 MB limit" });
    var form = await context.Request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    if (file == null)
        return Results.BadRequest(new { error = "No file uploaded" });

    using var stream = file.OpenReadStream();
    var ms = new MemoryStream();
    await stream.CopyToAsync(ms);
    ms.Position = 0;

    try
    {
        await logService.LoadLogAsync(ms);
        return Results.Ok(new
        {
            frameCount = logService.FrameCount,
            distinctFrameIds = (await logService.GetDistinctFrameIdsAsync()).ToList()
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/log/decode-signal", async (uint frameId, string signalName, CanLogService logService, DbcEditService editService) =>
{
    if (!logService.IsLoaded)
        return Results.BadRequest(new { error = "No log file loaded" });
    if (!editService.IsLoaded)
        return Results.BadRequest(new { error = "No DBC file loaded" });

    var defn = await editService.GetMessageDefinitionAsync(frameId);
    if (defn == null)
        return Results.NotFound(new { error = $"No message definition for frame ID 0x{frameId:X}" });

    var signalDef = defn.Signals.FirstOrDefault(s => s.Signal.Name == signalName);
    if (signalDef == null)
        return Results.NotFound(new { error = $"Signal '{signalName}' not found in message 0x{frameId:X}" });

    const int maxDecodeFrames = 10000;
    var frames = await logService.GetFramesAsync(0, maxDecodeFrames, frameId);
    var timeSeries = new List<object>();
    foreach (var frame in frames)
    {
        var bitArray = new BitArray(frame.FramePayload);
        MessageValue? value = null;
        var success = defn.IsMultiplexed
            ? Decoder.TryParseMultiplexedMessage(defn, bitArray, out value)
            : Decoder.TryParseRegularMessage(defn, bitArray, out value);
        if (!success || value == null) continue;

        var signalValue = value.Signals.FirstOrDefault(s => s.SignalName == signalName);
        if (signalValue == null) continue;
        timeSeries.Add(new { time = frame.FrameTime, value = signalValue.Value });
    }

    return Results.Ok(timeSeries);
});

app.MapGet("/api/log/frames", async (int skip, int take, uint? frameId, CanLogService logService, DbcEditService editService) =>
{
    if (!logService.IsLoaded)
        return Results.BadRequest(new { error = "No log file loaded" });

    take = Math.Clamp(take, 1, 10000);

    var frames = await logService.GetFramesAsync(skip, take, frameId);
    var result = new List<object>();
    var index = 0;
    foreach (var f in frames)
    {
        var defn = await editService.GetMessageDefinitionAsync(f.FrameId);
        result.Add(new
        {
            index = skip + index,
            frameTime = f.FrameTime,
            frameId = f.FrameId,
            busId = f.BusId,
            payload = f.FramePayload,
            messageName = defn?.Header.Name
        });
        index++;
    }
    return Results.Ok(result);
});

app.MapGet("/api/log/raw-frames", async (uint frameId, int skip, int take, CanLogService logService) =>
{
    if (!logService.IsLoaded)
        return Results.BadRequest(new { error = "No log file loaded" });

    take = Math.Clamp(take, 1, 10000);

    var frames = await logService.GetFramesAsync(skip, take, frameId);
    var result = frames.Select(f => new
    {
        frameTime = f.FrameTime,
        frameId = f.FrameId,
        busId = f.BusId,
        payload = f.FramePayload
    });
    return Results.Ok(result);
});

app.MapGet("/api/log/frames/{index}", async (int index, CanLogService logService, DbcEditService editService) =>
{
    if (!logService.IsLoaded)
        return Results.BadRequest(new { error = "No log file loaded" });

    try
    {
        var frame = await logService.GetFrameAsync(index);
        var defn = await editService.GetMessageDefinitionAsync(frame.FrameId);
        if (defn == null)
            return Results.Ok(new
            {
                index,
                frameTime = frame.FrameTime,
                frameId = frame.FrameId,
                busId = frame.BusId,
                payload = frame.FramePayload,
                decoded = (object?)null
            });

        var bitArray = new BitArray(frame.FramePayload);
        var decoded = editService.IsLoaded
            ? Decoder.TryParseRegularMessage(defn, bitArray, out var value) || (defn.IsMultiplexed && Decoder.TryParseMultiplexedMessage(defn, bitArray, out value))
                ? (object?)new
                {
                    messageName = value!.MessageName,
                    multiplexValue = value.MultiplexValue,
                    signals = value.Signals.Select(s => new
                    {
                        name = s.SignalName,
                        value = s.Value,
                        unit = defn.Signals.FirstOrDefault(ds => ds.Signal.Name == s.SignalName)?.Signal.Unit ?? ""
                    })
                }
                : null
            : null;

        return Results.Ok(new
        {
            index,
            frameTime = frame.FrameTime,
            frameId = frame.FrameId,
            busId = frame.BusId,
            payload = frame.FramePayload,
            decoded
        });
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/decode/preview", async (PreviewRequest request, CanLogService logService, DbcEditService editService) =>
{
    if (!logService.IsLoaded)
        return Results.BadRequest(new { error = "No log file loaded" });
    if (!editService.IsLoaded)
        return Results.BadRequest(new { error = "No DBC file loaded" });

    try
    {
        var frame = await logService.GetFrameAsync(request.FrameIndex);
        var message = await editService.GetMessageAsync(frame.FrameId);
        if (message == null)
            return Results.NotFound(new { error = $"No message definition for frame ID 0x{frame.FrameId:X}" });

        var overrides = request.SignalOverrides.ToDictionary(
            kvp => kvp.Key,
            kvp =>
            {
                var existing = message.Signals.FirstOrDefault(s => s.Name == kvp.Key);
                if (existing == null)
                    throw new KeyNotFoundException($"Signal '{kvp.Key}' not found in message 0x{frame.FrameId:X}");

                return kvp.Value.ApplyTo(existing);
            }
        );

        try
        {
            var defn = await editService.BuildDefinitionWithOverridesAsync(frame.FrameId, overrides);
            var bitArray = new BitArray(frame.FramePayload);
            MessageValue? value = null;
            var success = defn.IsMultiplexed
                ? Decoder.TryParseMultiplexedMessage(defn, bitArray, out value)
                : Decoder.TryParseRegularMessage(defn, bitArray, out value);

            if (!success || value == null)
                return Results.Ok(new { decoded = (object?)null });

            return Results.Ok(new
            {
                messageName = value.MessageName,
                multiplexValue = value.MultiplexValue,
                signals = value.Signals.Select(s => new
                {
                    name = s.SignalName,
                    value = s.Value,
                    unit = defn.Signals.FirstOrDefault(ds => ds.Signal.Name == s.SignalName)?.Signal.Unit ?? ""
                })
            });
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new { error = "Validation failed", details = string.Join("; ", ex.Errors) });
        }
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

void LogWs(string message) => Console.WriteLine($"[WS/Pandas] {message}");

app.MapGet("/ws/pandas", async (HttpContext context, PandasConnectionService pandas) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        LogWs($"Rejected - not a WebSocket request");
        context.Response.StatusCode = 400;
        return;
    }

    LogWs($"Accepting WebSocket from {context.Connection.RemoteIpAddress}");
    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    LogWs($"WebSocket accepted. State: {webSocket.State}");
    var clientId = pandas.AddClient(webSocket);
    LogWs($"Client {clientId} added. Total clients: {pandas.ClientCount}");

    var buffer = new byte[4096];
    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30));
        while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
        {
            var ms = new MemoryStream();
            System.Net.WebSockets.WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(buffer, cts.Token);
                if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                    break;
                ms.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);

            if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                break;

            if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
            {
                var json = System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                try
                {
                    var cmd = System.Text.Json.JsonDocument.Parse(json);
                    var type = cmd.RootElement.GetProperty("type").GetString();
                    if (type == "track")
                    {
                        foreach (var id in cmd.RootElement.GetProperty("frameIds").EnumerateArray())
                            await pandas.TrackAsync((uint)id.GetInt32());
                    }
                    else if (type == "untrack")
                    {
                        foreach (var id in cmd.RootElement.GetProperty("frameIds").EnumerateArray())
                            await pandas.UntrackAsync((uint)id.GetInt32());
                    }
                }
                catch (Exception ex) { LogWs($"Client {clientId} message parse error: {ex.Message}"); }
            }
        }
    }
    catch (OperationCanceledException) { LogWs($"Client {clientId} timeout"); }
    catch (System.Net.WebSockets.WebSocketException ex) { LogWs($"Client {clientId} WebSocket error: {ex.Message}"); }
    finally
    {
        LogWs($"Client {clientId} disconnecting. State: {webSocket.State}");
        pandas.RemoveClient(clientId);
    }
});

app.MapPost("/api/pandas/connect", async (PandasConnectRequest req, PandasConnectionService pandas) =>
{
    try
    {
        await pandas.ConnectAsync(req.Ip, req.Port);
        return Results.Ok(new { connected = true, ip = req.Ip, port = req.Port });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { connected = false, error = ex.Message });
    }
});

app.MapPost("/api/pandas/disconnect", async (PandasConnectionService pandas) =>
{
    await pandas.DisconnectAsync();
    return Results.Ok(new { connected = false });
});

app.MapGet("/api/pandas/status", (PandasConnectionService pandas) =>
{
    var info = pandas.Info;
    return Results.Ok(new
    {
        connected = pandas.IsConnected,
        ip = info?.Ip,
        port = info?.Port,
        driverType = info?.DriverType
    });
});

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }

public record SignalUpdate(
    string? Name,
    int? StartBit,
    int? Size,
    DanubeRS.CanServerUtils.Lib.Parser.Endianness? Order,
    DanubeRS.CanServerUtils.Lib.Parser.ValueType? ValueType,
    double? Factor,
    double? Offset,
    double? Min,
    double? Max,
    string? Unit,
    string? Receiver,
    DanubeRS.CanServerUtils.Lib.Parser.Multiplex? Multiplex,
    bool ClearMultiplex = false);

public record SignalCreate(
    string Name,
    int StartBit,
    int Size,
    DanubeRS.CanServerUtils.Lib.Parser.Endianness? Order,
    DanubeRS.CanServerUtils.Lib.Parser.ValueType? ValueType,
    double? Factor,
    double? Offset,
    double? Min,
    double? Max,
    string? Unit,
    string? Receiver,
    DanubeRS.CanServerUtils.Lib.Parser.Multiplex? Multiplex);

public record MessageCreate(
    uint FrameId,
    string Name,
    int Size,
    string? Node);

public record PreviewRequest(
    int FrameIndex,
    Dictionary<string, SignalOverride> SignalOverrides);

public record SignalOverride(
    int? StartBit,
    int? Size,
    DanubeRS.CanServerUtils.Lib.Parser.Endianness? Order,
    DanubeRS.CanServerUtils.Lib.Parser.ValueType? ValueType,
    double? Factor,
    double? Offset,
    double? Min,
    double? Max,
    string? Unit)
{
    public DanubeRS.CanServerUtils.Lib.Parser.Signal ApplyTo(DanubeRS.CanServerUtils.Lib.Parser.Signal existing)
    {
        return existing with
        {
            StartBit = StartBit ?? existing.StartBit,
            Size = Size ?? existing.Size,
            Order = Order ?? existing.Order,
            ValueType = ValueType ?? existing.ValueType,
            Factor = Factor ?? existing.Factor,
            Offset = Offset ?? existing.Offset,
            Min = Min ?? existing.Min,
            Max = Max ?? existing.Max,
            Unit = Unit ?? existing.Unit
        };
    }
}

public record PandasConnectRequest(string Ip, int Port = 1338);