using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Start the Libplanet node with settings.")]
internal sealed class StartCommand : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandPropertyRequired]
    [CommandSummary("The path of the repository.")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.Exist)]
    public string RepositoryPath { get; set; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the REPL is not started.")]
    public bool NoREPL { get; init; }

    [CommandPropertySwitch("manual-start", 'm')]
    [CommandSummary("If set, the node does not start automatically. " +
                    "Instead, it waits for the user to start it manually.")]
    public bool ManualStart { get; init; } = false;

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        options.ParentProcessId = ParentProcessId;
        options.NoREPL = NoREPL;
    }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var application = new Application(RepositoryPath);
            application.Services.AddSingleton<IConfigureOptions<ApplicationOptions>>(this);
            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }
}
