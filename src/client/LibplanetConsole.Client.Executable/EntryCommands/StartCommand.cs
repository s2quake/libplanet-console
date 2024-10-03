using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using LibplanetConsole.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Executable.EntryCommands;

[CommandSummary("Start the Libplanet client with settings.")]
internal sealed class StartCommand : CommandAsyncBase
{
    private static readonly ApplicationSettingsCollection _settingsCollection = new();

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

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var settingsPath = Path.Combine(RepositoryPath, Repository.SettingsFileName);
            var serviceCollection = new ApplicationServiceCollection(_settingsCollection);
            var applicationSettings = Load(settingsPath) with
            {
                ParentProcessId = ParentProcessId,
                NoREPL = NoREPL,
            };
            var applicationOptions = applicationSettings.ToOptions();

            serviceCollection.AddSingleton(applicationOptions);
            serviceCollection.AddClientApplication<Application>(applicationOptions);
            serviceCollection.AddClientExecutable();

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var @out = Console.Out;
            await using var application = serviceProvider.GetRequiredService<Application>();
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
