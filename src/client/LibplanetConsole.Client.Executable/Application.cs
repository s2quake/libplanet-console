using JSSoft.Commands;
using LibplanetConsole.Client.Executable.Commands;
using LibplanetConsole.Client.Executable.Tracers;
using LibplanetConsole.Logging;

namespace LibplanetConsole.Client.Executable;

internal sealed class Application
{
    private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();
    private readonly LoggingFilter[] _filters =
    [
        new PrefixFilter("app", "LibplanetConsole."),
    ];

    private readonly LoggingFilter[] _traceFilters =
    [
        new PrefixFilter("app", "LibplanetConsole."),
    ];

    public Application()
    {
        // var port = options.Port;
        var services = _builder.Services;
        var configuration = _builder.Configuration;
        // foreach (var instance in instances)
        // {
        //     services.AddSingleton(instance.GetType(), instance);
        // }

        _builder.WebHost.ConfigureKestrel(options =>
        {
            // options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
            // options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        });

        // if (options.LogPath != string.Empty)
        // {
        //     services.AddLogging(options.LogPath, "client.log", _filters);
        // }
        // else
        // {
        //     services.AddLogging(_traceFilters);
        // }

        services.AddSingleton<CommandContext>();
        services.AddSingleton<SystemTerminal>();

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddClient(configuration);

        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddHostedService<BlockChainEventTracer>();
        services.AddHostedService<ClientEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var app = _builder.Build();

        app.UseClient();
        app.MapGet("/", () => "Libplanet-Client");
        app.MapGrpcReflectionService().AllowAnonymous();

        await Console.Out.WriteLineAsync();
        await app.RunAsync(cancellationToken);
    }
}
