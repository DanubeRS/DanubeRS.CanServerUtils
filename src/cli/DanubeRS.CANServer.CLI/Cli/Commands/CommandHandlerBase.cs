using System.CommandLine.Invocation;

namespace DanubeRS.CANServer.CLI.Commands;

public abstract class CommandHandlerBase : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        var cancellationToken = context.GetCancellationToken();
        return Run(context, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var cancellationToken = context.GetCancellationToken();
        return await Run(context, cancellationToken);
    }

    protected abstract Task<int> Run(InvocationContext context, CancellationToken cancellationToken);
}