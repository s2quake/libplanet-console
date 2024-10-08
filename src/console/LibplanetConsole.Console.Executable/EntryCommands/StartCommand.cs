using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using LibplanetConsole.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Run the libplanet-console using the given repository path.")]
internal sealed class StartCommand : CommandAsyncBase
{
    private static readonly ApplicationSettingsCollection _settingsCollection = new();

    [CommandPropertyRequired]
    [CommandSummary("The path of the repository.")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.Exist)]
    public string RepositoryPath { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var resolver = new RepositoryPathResolver();
        var repositoryPath = Path.GetFullPath(RepositoryPath);
        var settingsPath = resolver.GetSettingsPath(repositoryPath);
        var applicationSettings = Load(settingsPath);
        var applicationOptions = applicationSettings.ToOptions() with
        {
            Nodes = Repository.LoadNodeOptions(repositoryPath, resolver),
            Clients = Repository.LoadClientOptions(repositoryPath, resolver),
            LogPath = applicationSettings.LogPath,
            LibraryLogPath = applicationSettings.LibraryLogPath,
        };
        var serviceCollection = new ApplicationServiceCollection(_settingsCollection);

        serviceCollection.AddConsole(applicationOptions);
        serviceCollection.AddApplication(applicationOptions);

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var @out = System.Console.Out;
        await using var application = serviceProvider.GetRequiredService<Application>();
        await @out.WriteLineAsync();
        await application.RunAsync();
        await @out.WriteLineAsync("\u001b0");
    }

    private static ApplicationSettings Load(string settingsPath)
    {
        SettingsLoader.Load(settingsPath, _settingsCollection.ToDictionary());
        return _settingsCollection.Peek<ApplicationSettings>();
    }
}
