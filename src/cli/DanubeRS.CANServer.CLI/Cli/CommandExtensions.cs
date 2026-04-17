using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace DanubeRS.CANServer.CLI;

public static class CommandExtensions
{
    public static void SetScopedAction<T>(this Command command, IServiceProvider provider)
        where T : ICommandAction
    {
        command.SetAction(async (parseResult, ct) =>
        {
            await using var scope = provider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<T>();
            return await handler.InvokeAsync(parseResult, ct);
        });
    }
}
