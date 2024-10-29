using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Evidence;
using LibplanetConsole.Console.Executable.Commands;
using LibplanetConsole.Console.Executable.Tracers;
using LibplanetConsole.Logging;
using Serilog;

namespace LibplanetConsole.Console.Executable;

internal sealed class Application
{
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

    private readonly WebApplicationBuilder _builder;

    public Application(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });

        services.AddSingleton<CommandContext>();
        services.AddSingleton<SystemTerminal>();
        services.AddSingleton<IInfoProvider, ServerInfoProvider>();

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddConsole(configuration);
        services.AddEvidence();

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggingInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddHostedService<ClientCollectionEventTracer>();
        services.AddHostedService<NodeCollectionEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();

        services.PostConfigure<ApplicationOptions>(options =>
        {
            var logPath = options.LogPath;
            if (logPath != string.Empty)
            {
                LoggerUtility.CreateLogger(logPath, "console.log", _filters);
            }
            else
            {
                LoggerUtility.CreateLogger(_traceFilters);
            }
        });
        _builder = builder;
    }

    public IServiceCollection Services => _builder.Services;

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
