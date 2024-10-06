using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using LibplanetConsole.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
            var builder = WebApplication.CreateBuilder();

            var settingsPath = Path.Combine(RepositoryPath, Repository.SettingsFileName);
            var applicationSettings = Load(settingsPath) with
            {
                ParentProcessId = ParentProcessId,
                NoREPL = NoREPL,
            };
            var applicationOptions = applicationSettings.ToOptions();

            foreach (var settings in _settingsCollection)
            {
                builder.Services.AddSingleton(settings.GetType(), settings);
            }

            var (_, port) = EndPointUtility.GetHostAndPort(applicationOptions.EndPoint);
            builder.WebHost.ConfigureKestrel(options =>
            {
                // Setup a HTTP/2 endpoint without TLS.
                options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });

            builder.Services.AddClient(applicationOptions);
            builder.Services.AddApplication(applicationOptions);

            // builder.Services.AddGrpc();

            using var app = builder.Build();

            app.MapGet("/", () => "123");
            // app.UseAuthentication();
            // app.UseAuthorization();

            var @out = Console.Out;
            await @out.WriteLineAsync();
            await app.RunAsync(cancellationToken);
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
