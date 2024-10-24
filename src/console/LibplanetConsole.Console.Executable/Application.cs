using JSSoft.Commands;
using LibplanetConsole.Console.Evidence;
using LibplanetConsole.Console.Executable.Commands;
using LibplanetConsole.Console.Executable.Tracers;
using LibplanetConsole.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace LibplanetConsole.Console.Executable;

internal sealed class Application
{
    private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();
    private readonly LoggingFilter[] _filters =
    [
        new SourceContextFilter(
            "app.log",
            s => s.StartsWith("LibplanetConsole.") && !s.StartsWith("LibplanetConsole.Seed.")),
        new PrefixFilter("seed.log", "LibplanetConsole.Seed."),
    ];

    private readonly LoggingFilter[] _traceFilters =
    [
        new PrefixFilter("app", "LibplanetConsole."),
    ];

    public Application(ApplicationOptions options, object[] instances)
    {
        var port = options.Port;
        var services = _builder.Services;

        foreach (var instance in instances)
        {
            services.AddSingleton(instance.GetType(), instance);
        }

        _builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
            options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        });

        if (options.LogPath != string.Empty)
        {
            services.AddLogging(options.LogPath, "console.log", _filters);
        }
        else
        {
            services.AddLogging(_traceFilters);
        }

        services.AddSingleton<CommandContext>();
        services.AddSingleton<SystemTerminal>();

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddConsole(options);
        services.AddEvidence();

        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddHostedService<ClientCollectionEventTracer>();
        services.AddHostedService<NodeCollectionEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var app = _builder.Build();

        app.UseConsole();
        app.MapGet("/", () => "Libplanet-Console");
        app.MapGrpcReflectionService().AllowAnonymous();

        await System.Console.Out.WriteLineAsync();
        await app.RunAsync(cancellationToken);
    }
}
