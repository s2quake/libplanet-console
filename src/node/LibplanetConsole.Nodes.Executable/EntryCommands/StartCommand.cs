using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Settings;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

[CommandSummary("Start the Libplanet node with settings.")]
internal sealed class StartCommand : CommandAsyncBase
{
    private static readonly ApplicationSettingsCollection _settingsCollection = new();

    [CommandPropertyRequired]
    [Path(Type = PathType.File, ExistsType = PathExistsType.Exist)]
    public string SettingsPath { get; set; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    [Category("Hidden")]
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
            var settingsPath = Path.GetFullPath(SettingsPath);
            var components = _settingsCollection.ToArray();
            var applicationSettings = Load(settingsPath) with
            {
                ParentProcessId = ParentProcessId,
                NoREPL = NoREPL,
            };
            var applicationOptions = applicationSettings.ToOptions(components);
            var @out = Console.Out;
            await using var application = new Application(applicationOptions);
            await @out.WriteLineAsync();
            await application.RunAsync();
            await @out.WriteLineAsync("\u001b0");
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }

    private static ApplicationSettings Load(string settingsPath)
    {
        SettingsLoader.Load(settingsPath, _settingsCollection.ToDictionary());
        return _settingsCollection.Peek<ApplicationSettings>();
    }
}
