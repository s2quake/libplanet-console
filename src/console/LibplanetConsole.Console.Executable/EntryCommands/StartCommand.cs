using JSSoft.Commands;
using LibplanetConsole.DataAnnotations;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Runs the libplanet-console using the given repository path")]
internal sealed class StartCommand : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the path of the repository")]
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
            options.GenesisPath = GetFullPath(options.GenesisPath);
            options.AppProtocolVersionPath = GetFullPath(options.AppProtocolVersionPath);
            options.LogPath = GetFullPath(options.LogPath);
            options.AliasPath = GetFullPath(options.AliasPath);
            options.ActionProviderModulePath = GetFullPath(options.ActionProviderModulePath);
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
            var application = new Application(builder)
            {
                RepositoryPath = RepositoryPath,
            };
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
