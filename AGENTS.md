# DanubeRS.CANServer - Agent Instructions

## Project Overview

.NET 8.0/9.0 utility for reading, decoding, and exporting binary CANServer logs. Uses Superpower parser combinator library for DBC file parsing and Microsoft.Extensions for dependency injection/logging.

## Build & Test Commands

```bash
# Build entire solution
dotnet build src/DanubeRS.CANServer.sln

# Run all tests
dotnet test src/lib/DanubeRS.DBCParser.Tests/DanubeRS.DBCParser.Tests.csproj

# Run single test by name
dotnet test --filter "FullyQualifiedName~ValidPacketData_Parsed_ValidOutputData"

# Run tests in specific class
dotnet test --filter "FullyQualifiedName~DecodeTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Build and run CLI
dotnet run --project src/cli/DanubeRS.CANServer.CLI/DanubeRS.CANServer.CLI.csproj
```

## Project Structure

- `src/lib/` - Core libraries (DBCParser, CANServer.Lib)
- `src/cli/` - Command-line applications
- `src/dash/` - Dashboard applications (Android/Browser)

## Code Style & Conventions

### Imports
- System namespaces first (alphabetically grouped)
- Third-party namespaces second
- Project namespaces last (`DanubeRS.*`)
- Use global usings for test frameworks (`global using Xunit;`)

### Formatting
```csharp
// Class declarations with primary constructors
public class Database(ILogger<Database> logger)

// Records for immutable data structures
public record SignalValue(string SignalName, double Value);

// Lambda expressions use explicit types where beneficial
defn.Signals.OrderBy(s => s.Signal.StartBit).Select(signal => GetSignalValue(data, signal))

// Pattern matching with switch expressions
frameLength = frameLength switch
{
    < 0 => 0,
    > 8 => 8,
    _ => frameLength
};
```

### Naming Conventions
- Types: PascalCase (`Database`, `LogReaderFrame`)
- Methods/Properties: PascalCase (`TryParseBinaryMessage`, `GetNextFrame`)
- Private fields: lowercase with underscore prefix (`_messageDefn`, `_logger`)
- Constants: PascalCase with `const` keyword (`HeaderSize = 22`)
- Test methods: Scenario_State_Result pattern (`ValidPacketData_Parsed_ValidOutputData`)

### Types & Nullability
- `<Nullable>enable</Nullable>` is enabled project-wide
- Use `[NotNullWhen(true)]` for output parameters in Try-patterns
- Prefer `record` over `class` for immutable data carriers
- Use `interface` for abstraction with implementation injection
- Favor explicit types over var at method scope

### Error Handling
```csharp
// Return nullable or bool for expected failure cases
public static async Task<Reader?> Open(Stream stream, ILogger logger)
{
    return await reader.Initialise() ? reader : null;
}

// Try-pattern with output parameters
public bool TryParseBinaryMessage(uint frameId, byte[] data, 
    [NotNullWhen(true)] out MessageValue? value,
    [NotNullWhen(true)] out MessageDefinition? defn)

// Throw for unexpected conditions
if (response == null)
    throw new NullReferenceException("Unable to get logging status");

// Catch specific exceptions, rethrow with context
catch (EndOfStreamException e)
{
    throw new Exception("Time sync payload is the wrong length", e);
}
```

### Dependency Injection
- Use constructor injection for all dependencies
- Register services as scoped in CLI applications
- Factory patterns for runtime-created instances:
```csharp
collection.AddScoped<DownloaderFactory>(provider => uri => 
{
    // factory implementation
});
```

### Testing Patterns
- xUnit with `[Fact]` and `[Theory]` attributes
- FluentAssertions for assertions (`.Should().BeTrue()`)
- Use `NullLogger<T>` for testing without logging setup
- Test class naming: `{ClassName}Tests` (`DecodeTests`)
- Arrange-Act-Assert pattern in test methods

### Logging
```csharp
// Inject ILogger<T> via constructor
public class Database(ILogger<Database> logger)

// Check log level before writing
if (logger.IsEnabled(LogLevel.Trace))
    logger.LogTrace("Data\n{BitArrayData}", GetBitArrayDataForLog(bitArray));
```

### Async/Await
- Use `async Task<T>` for async methods returning values
- Propagate `CancellationToken` through async chains
- Use `await using` for disposable async resources
- Prefer `ReadAsync`/`WriteAsync` for I/O operations

## Key Dependencies
- **Superpower** - Parser combinator library for DBC parsing
- **FluentAssertions** - Fluent assertion library for tests
- **Microsoft.Extensions.Logging** - Logging abstraction
- **System.CommandLine** - CLI framework with hosting support
- **Polly.Core** - Resilience and retry patterns

## Architecture Notes
- Tokenizer/Parser pattern for DBC file parsing (Superpower)
- Factory interfaces for runtime configuration (`DownloaderFactory`, `ReaderFactory`)
- Command-handler pattern for CLI operations
- Multiplexed CAN message handling with switch values

## Worktree Usage

### Creating a Feature Worktree

For isolated development work, create a git worktree:

```bash
# Create new feature branch in worktree
git worktree add .worktrees/my-feature-name -b my-feature-name origin/master

# Enter the worktree directory
cd .worktrees/my-feature-name

# Do your work...
git commit -m "Implement feature"
git push -u origin my-feature-name

# When done, return to main and remove worktree
cd ../..
git worktree remove .worktrees/my-feature-name
```

### Worktree Directory Structure

- `.worktrees/` - Contains isolated development branches
- Each subdirectory is a full git working copy with its own branch
- Safe to delete when feature is merged or abandoned

### When to Use Worktrees

- Refactoring work that spans multiple commits
- Experimental features that shouldn't block main branch
- Parallel development on unrelated tasks
