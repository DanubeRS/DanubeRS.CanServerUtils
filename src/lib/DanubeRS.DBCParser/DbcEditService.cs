using System.Text;
using DanubeRS.DBCParser.Parser;
using DanubeRS.DBCParser.Tokenizer;
using DanubeRS.DBCParser;
using Microsoft.Extensions.Logging;

namespace DanubeRS.DBCParser;

public class DbcEditService
{
    private readonly ILogger<DbcEditService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private ParsedDb? _db;
    private readonly Dictionary<uint, Database.MessageDefinition> _definitionCache = new();

    public DbcEditService(ILogger<DbcEditService> logger)
    {
        _logger = logger;
    }

    public bool IsLoaded => _db != null;

    public ParsedDb CurrentDb => _db ?? throw new InvalidOperationException("No DBC file loaded");

    public async Task LoadDbcAsync(Stream stream)
    {
        await _lock.WaitAsync();
        try
        {
            using var reader = new StreamReader(stream);
            var dbcString = await reader.ReadToEndAsync();

            var tokens = DbcTokenizer.Instance.Tokenize(dbcString);
            var parsed = DbcParser.Database(tokens);

            if (!parsed.HasValue)
            {
                var errors = parsed.ErrorMessage != null
                    ? [parsed.ErrorMessage]
                    : parsed.Expectations != null
                        ? parsed.Expectations
                        : ["Unknown parse error"];
                throw new DbcParseException(errors);
            }

            _db = parsed.Value;
            RebuildAllDefinitions();
            _logger.LogInformation("Loaded DBC with {MessageCount} messages", _db.Messages.Length);
        }
        finally
        {
            _lock.Release();
        }
    }

    public void LoadDbc(string dbcContent)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(dbcContent));
        LoadDbcAsync(stream).GetAwaiter().GetResult();
    }

    public async Task<string> ExportDbcAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_db == null)
                throw new InvalidOperationException("No DBC file loaded");

            return DbcSerializer.Serialize(_db);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return CurrentDb.Messages;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Message?> GetMessageAsync(uint frameId)
    {
        await _lock.WaitAsync();
        try
        {
            return CurrentDb.Messages.FirstOrDefault(m => m.Header.Id == frameId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Database.MessageDefinition?> GetMessageDefinitionAsync(uint frameId)
    {
        await _lock.WaitAsync();
        try
        {
            if (_db == null)
                return null;

            if (_definitionCache.TryGetValue(frameId, out var defn))
                return defn;

            var message = CurrentDb.Messages.FirstOrDefault(m => m.Header.Id == frameId);
            if (message == null)
                return null;

            defn = new Database.MessageDefinition(message, _db);
            _definitionCache[frameId] = defn;
            return defn;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Signal> UpdateSignalAsync(uint frameId, string signalName, Signal updated)
    {
        await _lock.WaitAsync();
        try
        {
            var db = CurrentDb;
            var messageIndex = Array.FindIndex(db.Messages, m => m.Header.Id == frameId);
            if (messageIndex < 0)
                throw new KeyNotFoundException($"Message with frame ID 0x{frameId:X} not found");

            var message = db.Messages[messageIndex];
            var signalIndex = Array.FindIndex(message.Signals, s => s.Name == signalName);
            if (signalIndex < 0)
                throw new KeyNotFoundException($"Signal '{signalName}' not found in message '{message.Header.Name}'");

            var validation = DbcValidator.ValidateSignalEdit(updated, message, message.Header.Size, signalName);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var newSignals = message.Signals.ToArray();
            newSignals[signalIndex] = updated;

            var newMessage = message with { Signals = newSignals };
            var newMessages = db.Messages.ToArray();
            newMessages[messageIndex] = newMessage;

            _db = db with { Messages = newMessages };
            InvalidateDefinition(frameId);

            _logger.LogInformation("Updated signal '{SignalName}' in message 0x{FrameId:X}", signalName, frameId);
            return updated;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Signal> AddSignalAsync(uint frameId, Signal newSignal)
    {
        await _lock.WaitAsync();
        try
        {
            var db = CurrentDb;
            var messageIndex = Array.FindIndex(db.Messages, m => m.Header.Id == frameId);
            if (messageIndex < 0)
                throw new KeyNotFoundException($"Message with frame ID 0x{frameId:X} not found");

            var message = db.Messages[messageIndex];

            var validation = DbcValidator.ValidateNewSignal(newSignal, message, message.Header.Size);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var newSignals = message.Signals.Concat([newSignal]).ToArray();
            var newMessage = message with { Signals = newSignals };
            var newMessages = db.Messages.ToArray();
            newMessages[messageIndex] = newMessage;

            _db = db with { Messages = newMessages };
            InvalidateDefinition(frameId);

            _logger.LogInformation("Added signal '{SignalName}' to message 0x{FrameId:X}", newSignal.Name, frameId);
            return newSignal;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RemoveSignalAsync(uint frameId, string signalName)
    {
        await _lock.WaitAsync();
        try
        {
            var db = CurrentDb;
            var messageIndex = Array.FindIndex(db.Messages, m => m.Header.Id == frameId);
            if (messageIndex < 0)
                throw new KeyNotFoundException($"Message with frame ID 0x{frameId:X} not found");

            var message = db.Messages[messageIndex];
            var signalIndex = Array.FindIndex(message.Signals, s => s.Name == signalName);
            if (signalIndex < 0)
                throw new KeyNotFoundException($"Signal '{signalName}' not found in message '{message.Header.Name}'");

            var newSignals = message.Signals.Where(s => s.Name != signalName).ToArray();
            var newMessage = message with { Signals = newSignals };
            var newMessages = db.Messages.ToArray();
            newMessages[messageIndex] = newMessage;

            _db = db with { Messages = newMessages };
            InvalidateDefinition(frameId);

            _logger.LogInformation("Removed signal '{SignalName}' from message 0x{FrameId:X}", signalName, frameId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Message> AddMessageAsync(MessageHeader header)
    {
        await _lock.WaitAsync();
        try
        {
            var db = CurrentDb;
            var validation = DbcValidator.ValidateNewMessage(header, db);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var newMessage = new Message(header, []);
            var newMessages = db.Messages.Concat([newMessage]).ToArray();

            _db = db with { Messages = newMessages };

            _logger.LogInformation("Added message '{MessageName}' (0x{FrameId:X})", header.Name, header.Id);
            return newMessage;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RemoveMessageAsync(uint frameId)
    {
        await _lock.WaitAsync();
        try
        {
            var db = CurrentDb;
            var messageIndex = Array.FindIndex(db.Messages, m => m.Header.Id == frameId);
            if (messageIndex < 0)
                throw new KeyNotFoundException($"Message with frame ID 0x{frameId:X} not found");

            var newMessages = db.Messages.Where(m => m.Header.Id != frameId).ToArray();
            _db = db with { Messages = newMessages };
            _definitionCache.Remove(frameId);

            _logger.LogInformation("Removed message 0x{FrameId:X}", frameId);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Message> UpdateMessageAsync(uint frameId, Message updated)
    {
        await _lock.WaitAsync();
        try
        {
            var db = CurrentDb;
            var messageIndex = Array.FindIndex(db.Messages, m => m.Header.Id == frameId);
            if (messageIndex < 0)
                throw new KeyNotFoundException($"Message with frame ID 0x{frameId:X} not found");

            var validation = DbcValidator.ValidateMessageEdit(updated, db);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var newMessages = db.Messages.ToArray();
            newMessages[messageIndex] = updated;

            _db = db with { Messages = newMessages };
            InvalidateDefinition(frameId);

            _logger.LogInformation("Updated message 0x{FrameId:X}", frameId);
            return updated;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Database.MessageDefinition> BuildDefinitionWithOverridesAsync(uint frameId, Dictionary<string, Signal> overrides)
    {
        await _lock.WaitAsync();
        try
        {
            var message = CurrentDb.Messages.FirstOrDefault(m => m.Header.Id == frameId)
                ?? throw new KeyNotFoundException($"Message with frame ID 0x{frameId:X} not found");

            var newSignals = message.Signals.Select(s =>
                overrides.TryGetValue(s.Name, out var overriden) ? overriden : s
            ).ToArray();

            var draftMessage = message with { Signals = newSignals };
            return new Database.MessageDefinition(draftMessage, CurrentDb);
        }
        finally
        {
            _lock.Release();
        }
    }

    private void InvalidateDefinition(uint frameId)
    {
        _definitionCache.Remove(frameId);
    }

    private void RebuildAllDefinitions()
    {
        _definitionCache.Clear();
        if (_db == null) return;

        foreach (var message in _db.Messages)
        {
            _definitionCache[message.Header.Id] = new Database.MessageDefinition(message, _db);
        }
    }
}

public class ValidationException(List<string> errors) : Exception(string.Join("; ", errors))
{
    public List<string> Errors { get; } = errors;
}