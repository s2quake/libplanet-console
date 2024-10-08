using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Executable;

internal sealed class TerminalHostedService(
    IHostApplicationLifetime applicationLifetime,
    CommandContext commandContext,
    SystemTerminal terminal) : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task _runningTask = Task.CompletedTask;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(async () =>
        {
            var message = "Welcome to console for Libplanet.";
            var sw = new StringWriter();
            commandContext.Out = sw;
            await System.Console.Out.WriteColoredLineAsync(message, TerminalColorType.BrightGreen);
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            await sw.WriteLineAsync();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            commandContext.Out = System.Console.Out;
            await System.Console.Out.WriteAsync(sw.ToString());

            _runningTask = terminal.StartAsync(_cancellationTokenSource.Token);
        });
        applicationLifetime.ApplicationStopping.Register(() =>
        {
            _cancellationTokenSource.Cancel();
        });

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _runningTask;
        await terminal.StopAsync(cancellationToken);
    }

    void IDisposable.Dispose() => _cancellationTokenSource.Dispose();
}
