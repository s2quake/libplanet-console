using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Executable.Tracers;

internal sealed class ClientEventTracer(IApplicationOptions options) : ClientContentBase
{
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        var endPoint = options.Port;
        var message = $"BlockChain has been started.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        var endPoint = options.Port;
        var message = $"BlockChain has been stopped.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
        return Task.CompletedTask;
    }
}
