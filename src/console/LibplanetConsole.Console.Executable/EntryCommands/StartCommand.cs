using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using LibplanetConsole.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
        try
        {
            var builder = WebApplication.CreateBuilder();

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

            foreach (var settings in _settingsCollection)
            {
                builder.Services.AddSingleton(settings.GetType(), settings);
            }

            var (_, port) = EndPointUtility.GetHostAndPort(applicationOptions.EndPoint);
            builder.WebHost.ConfigureKestrel(options =>
            {
                // Setup a HTTP/2 endpoint without TLS.
                options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                // options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });

            builder.Services.AddConsole(applicationOptions);
            builder.Services.AddExecutable(applicationOptions);

            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection();

            using var app = builder.Build();

            app.UseConsole();
            app.MapGet("/", () => "Libplanet Console");

            var @out = System.Console.Out;
            await @out.WriteLineAsync();
            await app.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(System.Console.Out);
            Environment.Exit(1);
        }
    }

    private static ApplicationSettings Load(string settingsPath)
    {
        SettingsLoader.Load(settingsPath, _settingsCollection.ToDictionary());
        return _settingsCollection.Peek<ApplicationSettings>();
    }
}
