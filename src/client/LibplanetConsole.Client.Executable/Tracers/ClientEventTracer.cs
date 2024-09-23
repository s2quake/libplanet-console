using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Executable.Tracers;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class ClientEventTracer(IClient client)
    : IApplicationService, IDisposable
{
    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        client.Started += Client_Started;
        client.Stopped += Client_Stopped;
        return Task.CompletedTask;
    }

    void IDisposable.Dispose()
    {
        client.Started -= Client_Started;
        client.Stopped -= Client_Stopped;
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        var message = $"Client has been started.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
        Console.Out.WriteLineAsJson(client.Info);
    }

    private void Client_Stopped(object? sender, StopEventArgs e)
    {
        var message = $"Client has been stopped: {e.Reason}.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}
