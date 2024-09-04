using JSSoft.Commands;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Executable.EntryCommands;

[CommandSummary("Start the Libplanet client with settings.")]
internal sealed class StartCommand : CommandAsyncBase
{
    private static readonly ApplicationSettingsCollection _settingsCollection = new();

    [CommandPropertyRequired]
    public string SettingsPath { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var components = _settingsCollection.ToArray();
            var applicationSettings = Load(SettingsPath);
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
