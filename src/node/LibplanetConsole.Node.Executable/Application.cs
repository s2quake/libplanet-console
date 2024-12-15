using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Logging;
using LibplanetConsole.Node.Bank;
using LibplanetConsole.Node.Delegation;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;
using LibplanetConsole.Node.Guild;
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
        services.AddSingleton<IAddressProvider, AddressProvider>();
        services.AddSingleton<ICurrencyProvider, CurrencyProvider>();

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddNode(configuration);
        services.AddExplorer(configuration);
        services.AddEvidence();
        services.AddBank();
        services.AddGuild();
        services.AddDelegation();

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggingInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddHostedService<BlockChainEventTracer>();
        services.AddHostedService<NodeEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();

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
        app.UseDelegation();
        app.UseGuild();
        app.MapGet("/", () => "Libplanet-Node");
        app.MapGrpcReflectionService().AllowAnonymous();

        await Console.Out.WriteLineAsync();
        await app.RunAsync(cancellationToken);
    }
}
