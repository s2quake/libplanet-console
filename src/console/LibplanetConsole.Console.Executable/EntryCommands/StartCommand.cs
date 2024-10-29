using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.DataAnnotations;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Run the libplanet-console using the given repository path.")]
internal sealed class StartCommand : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandPropertyRequired]
    [CommandSummary("The path of the repository.")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.Exist)]
    public string RepositoryPath { get; set; } = string.Empty;

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        var repositoryPath = Path.GetFullPath(RepositoryPath);
        var oldDirectory = Directory.GetCurrentDirectory();
        var resolver = new RepositoryPathResolver();
        try
        {
            Directory.SetCurrentDirectory(repositoryPath);
            options.Nodes = Repository.LoadNodeOptions(repositoryPath, resolver);
            options.Clients = Repository.LoadClientOptions(repositoryPath, resolver);
            options.LogPath = GetFullPath(options.LogPath);
        }
        finally
        {
            Directory.SetCurrentDirectory(oldDirectory);
        }
    }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(options: new()
            {
                ContentRootPath = RepositoryPath,
            });
            var services = builder.Services;
            var application = new Application(builder);
            services.AddSingleton<IConfigureOptions<ApplicationOptions>>(this);
            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(System.Console.Out);
            Environment.Exit(1);
        }
    }

    private static string GetFullPath(string path)
    {
        if (path != string.Empty && Path.IsPathRooted(path) is false)
        {
            return Path.GetFullPath(path);
        }

        return path;
    }
}
