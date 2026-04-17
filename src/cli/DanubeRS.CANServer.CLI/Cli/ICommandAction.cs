using System.CommandLine;

namespace DanubeRS.CANServer.CLI;

public interface ICommandAction
{
    Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken);
}
