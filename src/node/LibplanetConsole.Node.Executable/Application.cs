using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Logging;
using LibplanetConsole.Node.Bank;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;
using LibplanetConsole.Node.Hand;
using Microsoft.Extensions.Options;
using Serilog;

namespace LibplanetConsole.Node.Executable;

internal sealed class Application
{
    private static readonly LoggingFilter[] _filters =
    [
        new SourceContextFilter(
            "app.log",
            s => s.StartsWith("LibplanetConsole.") && !s.StartsWith("LibplanetConsole.Seed.")),
        new PrefixFilter("libplanet.log", "Libplanet."),
    ];

    private static readonly LoggingFilter[] _traceFilters =
    [
        new SourceContextFilter(
            "app.log",
            s => s.StartsWith("LibplanetConsole.") && !s.StartsWith("LibplanetConsole.Seed.")),
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
        services.AddSingleton<ICurrencyProvider, CurrencyProvider>();
        services.AddSingleton<ConsoleConfigureOptions>()
            .AddSingleton<IConfigureOptions<ApplicationOptions>>(
                s => s.GetRequiredService<ConsoleConfigureOptions>());

        services.AddSingleton<HelpCommand>()
            .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
            .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddNode(configuration);
        services.AddExplorer(configuration);
        services.AddEvidence();
        services.AddBank();
        services.AddHand();

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggingInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddHostedService<BlockChainEventTracer>();
        services.AddHostedService<NodeEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();
        services.AddHostedService<ConsoleHostedService>();

        services.PostConfigure<ApplicationOptions>(options =>
        {
            var logPath = options.LogPath;
            if (logPath != string.Empty)
            {
                LoggerUtility.CreateLogger(logPath, "node.log", _filters);
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

        app.UseNode();
        app.UseExplorer();
        app.UseEvidence();
        app.UseBank();
        app.UseHand();
        app.MapGet("/", () => "Libplanet-Node");
        app.MapGrpcReflectionService().AllowAnonymous();

        await System.Console.Out.WriteLineAsync();
        await app.RunAsync(cancellationToken);
    }
}
