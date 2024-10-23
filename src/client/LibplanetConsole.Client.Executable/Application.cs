using JSSoft.Commands;
using LibplanetConsole.Client.Executable.Commands;
using LibplanetConsole.Client.Executable.Tracers;
using LibplanetConsole.Logging;
using Serilog;

namespace LibplanetConsole.Client.Executable;

internal sealed class Application
{
    private readonly LoggingFilter[] _filters =
    [
        new PrefixFilter("app", "LibplanetConsole."),
    ];

    private readonly LoggingFilter[] _traceFilters =
    [
        new PrefixFilter("app", "LibplanetConsole."),
    ];

    private readonly WebApplicationBuilder _builder;

    public Application()
        : this(Create(null))
    {
    }

    public Application(string repositoryPath)
        : this(Create(repositoryPath))
    {
    }

    private Application(WebApplicationBuilder builder)
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

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddClient(configuration);

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggingInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddSingleton<IClientContent, ClientEventTracer>();
        services.AddHostedService<BlockChainEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();

        services.PostConfigure<ApplicationOptions>(options =>
       {
           var logPath = options.LogPath;
           if (logPath != string.Empty)
           {
               LoggerUtility.CreateLogger(logPath, "client.log", _filters);
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

        app.UseClient();
        app.MapGet("/", () => "Libplanet-Client");
        app.MapGrpcReflectionService().AllowAnonymous();

        await Console.Out.WriteLineAsync();
        await app.RunAsync(cancellationToken);
    }

    private static WebApplicationBuilder Create(string? repositoryPath)
    {
        var options = new WebApplicationOptions
        {
            ContentRootPath = repositoryPath,
        };

        return WebApplication.CreateBuilder(options);
    }
}
