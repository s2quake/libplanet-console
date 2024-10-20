using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using LibplanetConsole.Settings;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Start the Libplanet node with settings.")]
internal sealed class StartCommand : CommandAsyncBase
{
    private readonly ApplicationSettingsCollection _settingsCollection = new();

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

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var settingsPath = Path.Combine(RepositoryPath, Repository.SettingsFileName);
            var applicationSettings = Load(settingsPath) with
            {
                ParentProcessId = ParentProcessId,
                NoREPL = NoREPL,
            };
            var applicationOptions = applicationSettings.ToOptions();
            var application = new Application(applicationOptions, [.. _settingsCollection]);
            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }

    private ApplicationSettings Load(string settingsPath)
    {
        SettingsLoader.Load(settingsPath, _settingsCollection.ToDictionary());
        return _settingsCollection.Peek<ApplicationSettings>();
    }
}
