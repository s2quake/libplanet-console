using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Node.Executable.Extensions;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Starts the libplanet-node using the settings")]
internal sealed class StartCommand : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the path of the repository")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.Exist)]
    public string RepositoryPath { get; set; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console")]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the application starts without REPL")]
    public bool NoREPL { get; init; }

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        using var scope = DirectoryUtility.SetScopedDirectory(RepositoryPath);
        options.GenesisPath = GetFullPath(options.GenesisPath);
        options.AppProtocolVersionPath = GetFullPath(options.AppProtocolVersionPath);
        options.StorePath = GetFullPath(options.StorePath);
        options.LogPath = GetFullPath(options.LogPath);
        options.ActionProviderModulePath = GetFullPath(options.ActionProviderModulePath);
        options.ParentProcessId = ParentProcessId;
        options.NoREPL = NoREPL;
        options.EnsureActionProviderType(typeof(ActionProvider));
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
