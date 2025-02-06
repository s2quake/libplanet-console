using System.Diagnostics;
using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Threading;

namespace LibplanetConsole.Client.Executable;

internal sealed class SystemTerminalHostedService(
    IHostApplicationLifetime applicationLifetime,
    IServiceProvider serviceProvider,
    IApplicationOptions options,
    ILogger<SystemTerminalHostedService> logger)
    : IHostedService
{
    private Task _waitInputTask = Task.CompletedTask;
    private Task _waitForExitTask = Task.CompletedTask;
    private int _parentProcessId;
    private SystemTerminal? _terminal;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.NoREPL is false)
        {
            var terminal = serviceProvider.GetRequiredService<SystemTerminal>();
            var commandContext = serviceProvider.GetRequiredService<CommandContext>();
            var sw = new StringWriter();
            commandContext.Out = sw;
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken);
            await sw.WriteLineAsync();
            await commandContext.ExecuteAsync(args: [], cancellationToken);
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            commandContext.Out = System.Console.Out;
            await sw.WriteLineIfAsync(GetStartupCondition(options), GetStartupMessage());
            await System.Console.Out.WriteAsync(sw.ToString());

            await terminal.StartAsync(cancellationToken);
            _terminal = terminal;
        }
        else
        {
            _waitInputTask = WaitInputAsync();
        }

        if (options.ParentProcessId is not 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcessId = options.ParentProcessId;
            _waitForExitTask = WaitForExit(parentProcess);
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await TaskUtility.TryWaitAll(_waitInputTask, _waitForExitTask);
        if (_terminal is not null)
        {
            await _terminal.StopAsync(cancellationToken);
        }
    }

    private static bool GetStartupCondition(IApplicationOptions options)
    {
        if (options.HubUrl is not null)
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
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        applicationLifetime.ApplicationStopping.Register(cancellationTokenSource.Cancel);
        await System.Console.Out.WriteLineAsync("Press any key to exit.");
        await Task.Run(() => System.Console.ReadKey(intercept: true), cancellationToken);
        applicationLifetime.StopApplication();
    }

    private async Task WaitForExit(Process process)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        applicationLifetime.ApplicationStopping.Register(cancellationTokenSource.Cancel);
        await process.WaitForExitAsync(cancellationTokenSource.Token);
        logger.LogDebug("Parent process is exited: {ParentProcessId}.", _parentProcessId);
        applicationLifetime.StopApplication();
    }
}
