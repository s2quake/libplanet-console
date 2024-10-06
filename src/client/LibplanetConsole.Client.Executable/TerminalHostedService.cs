using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client.Executable;

internal sealed class TerminalHostedService(
    IHostApplicationLifetime applicationLifetime,
    CommandContext commandContext,
    SystemTerminal terminal,
    ApplicationOptions options) : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task _runningTask = Task.CompletedTask;
    private Task _waitInputTask = Task.CompletedTask;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.NoREPL != true)
        {
            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                var sw = new StringWriter();
                commandContext.Out = sw;
                await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
                await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
                await sw.WriteLineAsync();
                await commandContext.ExecuteAsync(args: [], cancellationToken: default);
                await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
                commandContext.Out = Console.Out;
                await sw.WriteLineIfAsync(GetStartupCondition(options), GetStartupMessage());
                await Console.Out.WriteAsync(sw.ToString());

                _runningTask = terminal.StartAsync(_cancellationTokenSource.Token);
            });
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _cancellationTokenSource.Cancel();
            });
        }
        else if (options.ParentProcessId == 0)
        {
            _waitInputTask = WaitInputAsync();
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _runningTask;
        await _waitInputTask;
        await terminal.StopAsync(cancellationToken);
    }

    void IDisposable.Dispose() => _cancellationTokenSource.Dispose();

    private static bool GetStartupCondition(ApplicationOptions options)
    {
        if (options.NodeEndPoint is not null)
        {
            return false;
        }

        return options.ParentProcessId == 0;
    }

    private static string GetStartupMessage()
    {
        var startText = TerminalStringBuilder.GetString("start", TerminalColorType.Green);
        return $"\nType '{startText}' to start the node.";
    }

    private async Task WaitInputAsync()
    {
        await Console.Out.WriteLineAsync("Press any key to exit.");
        await Task.Run(() => Console.ReadKey(intercept: true));
        applicationLifetime.StopApplication();
    }
}
