using System.Diagnostics;
using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Executable;

internal sealed class TerminalHostedService(
    IHostApplicationLifetime applicationLifetime,
    CommandContext commandContext,
    SystemTerminal terminal,
    ApplicationOptions options,
    ILogger<TerminalHostedService> logger)
    : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task _runningTask = Task.CompletedTask;
    private Task _waitInputTask = Task.CompletedTask;
    private Task _waitForExitTask = Task.CompletedTask;
    private int _parentProcessId;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.NoREPL is false)
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
        else if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcessId = options.ParentProcessId;
            _waitForExitTask = WaitForExit(parentProcess);
        }
        else if (options.ParentProcessId == 0)
        {
            _waitInputTask = WaitInputAsync();
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_runningTask, _waitInputTask, _waitForExitTask);
        await terminal.StopAsync(cancellationToken);
    }

    void IDisposable.Dispose() => _cancellationTokenSource.Dispose();

    private static bool GetStartupCondition(ApplicationOptions options)
    {
        if (options.SeedEndPoint is not null)
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

    private async Task WaitForExit(Process process)
    {
        await process.WaitForExitAsync();
        logger.LogDebug("Parent process is exited: {ParentProcessId}.", _parentProcessId);
        applicationLifetime.StopApplication();
    }
}
