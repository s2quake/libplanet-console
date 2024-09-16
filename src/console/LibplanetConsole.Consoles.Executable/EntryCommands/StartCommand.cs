using JSSoft.Commands;

namespace LibplanetConsole.Consoles.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console with repository path.")]
internal sealed class StartCommand : CommandAsyncBase
{
    [CommandPropertyRequired]
    [CommandSummary("The path of the repository.")]
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
        var @out = Console.Out;
        await using var application = new Application(applicationOptions);
        await @out.WriteLineAsync();
        await application.RunAsync();
        await @out.WriteLineAsync("\u001b0");
    }
}
