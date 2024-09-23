using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Executable;

internal sealed class Application(ApplicationOptions options)
    : ApplicationBase(options), IApplication
{
    private readonly ApplicationOptions _options = options;
    private Task _waitInputTask = Task.CompletedTask;

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        if (_options.NoREPL != true)
        {
            var sw = new StringWriter();
            var commandContext = this.GetRequiredService<CommandContext>();
            var terminal = this.GetRequiredService<SystemTerminal>();
            commandContext.Out = sw;
            await base.OnRunAsync(cancellationToken);
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            await sw.WriteLineAsync();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            await sw.WriteLineIfAsync(GetStartupCondition(_options), GetStartupMessage());
            await Console.Out.WriteAsync(sw.ToString());

            await terminal.StartAsync(cancellationToken);
        }
        else if (_options.ParentProcessId == 0)
        {
            await base.OnRunAsync(cancellationToken);
            _waitInputTask = WaitInputAsync();
        }
        else
        {
            await base.OnRunAsync(cancellationToken);
        }
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _waitInputTask;
    }

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
        Cancel();
    }
}
