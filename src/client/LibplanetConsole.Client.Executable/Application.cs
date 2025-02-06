using JSSoft.Commands;
using LibplanetConsole.Client.Bank;
using LibplanetConsole.Client.Delegation;
using LibplanetConsole.Client.Executable.Commands;
using LibplanetConsole.Client.Executable.Tracers;
using LibplanetConsole.Client.Guild;
using LibplanetConsole.Common;
using LibplanetConsole.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace LibplanetConsole.Client.Executable;

internal sealed class Application
{
    private readonly LoggingFilter[] _filters =
    [
        new PrefixFilter("app.log", "LibplanetConsole."),
    ];

    private readonly LoggingFilter[] _traceFilters =
    [
        new PrefixFilter("app.log", "LibplanetConsole."),
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
        services.AddSingleton<ConsoleConfigureOptions>()
            .AddSingleton<IConfigureOptions<ApplicationOptions>>(
                s => s.GetRequiredService<ConsoleConfigureOptions>());

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddClient(configuration);
        services.AddBank();
        services.AddDelegation();
        services.AddGuild();

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggingInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddSingleton<IClientContent, ClientEventTracer>();
        services.AddHostedService<BlockChainEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();
        services.AddHostedService<ConsoleHostedService>();

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
        app.UseBank();
        app.UseDelegation();
        app.UseGuild();
        app.MapGet("/", () => "Libplanet-Client");
        app.MapGrpcReflectionService().AllowAnonymous();

        await System.Console.Out.WriteLineAsync();
        await app.RunAsync(cancellationToken);
    }
}
