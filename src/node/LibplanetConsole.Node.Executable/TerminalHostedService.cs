using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Executable;

internal sealed class TerminalHostedService(
    IHostApplicationLifetime applicationLifetime,
    CommandContext commandContext,
    SystemTerminal terminal,
    ApplicationOptions _options) : IHostedService
{
    private Task _runningTask = Task.CompletedTask;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.NoREPL != true)
        {
            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                var sw = new StringWriter();
                commandContext.Out = sw;
                // await base.OnRunAsync(cancellationToken);
                await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
                await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
                await sw.WriteLineAsync();
                await commandContext.ExecuteAsync(args: [], cancellationToken: default);
                await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
                commandContext.Out = Console.Out;
                // await sw.WriteLineIfAsync(GetStartupCondition(_options), GetStartupMessage());
                await Console.Out.WriteAsync(sw.ToString());

                _runningTask = terminal.StartAsync(cancellationToken);
            });
        }
        else if (_options.ParentProcessId == 0)
        {
            // _waitInputTask = WaitInputAsync();
        }
        else
        {
            // await base.OnRunAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _runningTask;
        await terminal.StopAsync(cancellationToken);
    }

    // protected override async ValueTask OnDisposeAsync()
    // {
    //     await base.OnDisposeAsync();
    //     await _waitInputTask;
    // }

    // private static bool GetStartupCondition(ApplicationOptions options)
    // {
    //     if (options.SeedEndPoint is not null)
    //     {
    //         return false;
    //     }

    //     return options.ParentProcessId == 0;
    // }

    // private static string GetStartupMessage()
    // {
    //     var startText = TerminalStringBuilder.GetString("start", TerminalColorType.Green);
    //     return $"\nType '{startText}' to start the node.";
    // }

    // private async Task WaitInputAsync()
    // {
    //     await Console.Out.WriteLineAsync("Press any key to exit.");
    //     await Task.Run(() => Console.ReadKey(intercept: true));
    //     Cancel();
    // }
}
