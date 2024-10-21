using JSSoft.Commands;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;

namespace LibplanetConsole.Node.Executable;

internal sealed class Application
{
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

        services.AddSingleton<CommandContext>();
        services.AddSingleton<SystemTerminal>();

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddNode(configuration);
        services.AddExplorer(configuration);
        services.AddEvidence();

        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddHostedService<BlockChainEventTracer>();
        services.AddHostedService<NodeEventTracer>();
        services.AddHostedService<SystemTerminalHostedService>();
        _builder = builder;
    }

    public IServiceCollection Services => _builder.Services;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var app = _builder.Build();

        app.UseNode();
        app.UseExplorer();
        app.UseEvidence();
        app.MapGet("/", () => "Libplanet-Node");
        app.UseAuthentication();
        app.UseAuthorization();
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
