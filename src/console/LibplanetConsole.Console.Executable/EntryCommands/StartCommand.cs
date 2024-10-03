using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Run the libplanet-console using the given repository path.")]
internal sealed class StartCommand : CommandAsyncBase
{
    [CommandPropertyRequired]
    [CommandSummary("The path of the repository.")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.Exist)]
    public string RepositoryPath { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var repositoryPath = Path.GetFullPath(RepositoryPath);
        var resolver = new RepositoryPathResolver();
        var repository = Repository.Load(repositoryPath, resolver);
        var applicationOptions = new ApplicationOptions(repository.EndPoint)
        {
            Nodes = repository.Nodes,
            Clients = repository.Clients,
            Genesis = repository.Genesis,
            LogPath = repository.LogPath,
            LibraryLogPath = repository.LibraryLogPath,
        };
        var serviceCollection = new ApplicationServiceCollection();

        serviceCollection.AddConsole(applicationOptions);
        serviceCollection.AddApplication(applicationOptions);

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var @out = System.Console.Out;
        await using var application = serviceProvider.GetRequiredService<Application>();
        await @out.WriteLineAsync();
        await application.RunAsync();
        await @out.WriteLineAsync("\u001b0");
    }
}
