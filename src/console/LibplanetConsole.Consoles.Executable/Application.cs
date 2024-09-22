using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Consoles.Executable;

internal sealed partial class Application(ApplicationOptions options)
    : ApplicationBase(options), IApplication
{
    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        await base.OnRunAsync(cancellationToken);
        var message = "Welcome to console for Libplanet.";
        var sw = new StringWriter();
        var commandContext = this.GetRequiredService<CommandContext>();
        var terminal = this.GetRequiredService<SystemTerminal>();
        commandContext.Out = sw;
        await Console.Out.WriteColoredLineAsync(message, TerminalColorType.BrightGreen);
        await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
        await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
        await sw.WriteLineAsync();
        await commandContext.ExecuteAsync(args: [], cancellationToken: default);
        await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
        commandContext.Out = Console.Out;
        await Console.Out.WriteAsync(sw.ToString());
        await terminal.StartAsync(cancellationToken);
    }


    protected override Task OnServiceInitializeAsync(
        ApplicationServiceCollection serviceCollection,
        CancellationToken cancellationToken,
        IProgress<ProgressInfo> progress)
    {
        var commandProgress = new CommandProgress();
        return base.OnServiceInitializeAsync(serviceCollection, cancellationToken, commandProgress);
    }

}
