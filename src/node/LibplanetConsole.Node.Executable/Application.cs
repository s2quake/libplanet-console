using JSSoft.Commands;
using LibplanetConsole.Logging;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.Json;

namespace LibplanetConsole.Node.Executable;

internal sealed class Application
{
    private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();


    public Application()
    {

    }

    public Application(string repositoryPath)
    {
        // var port = options.Port;
        var options = new WebApplicationOptions
        {
            ContentRootPath = repositoryPath,
        };
        var builder = WebApplication.CreateBuilder(options);
        var services = builder.Services;
        var configuration = builder.Configuration;

        // _builder.WebHost.ConfigureKestrel(options =>
        // {
        //     options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
        //     options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        // });




        services.AddSingleton<CommandContext>();
        services.AddSingleton<SystemTerminal>();

        services.AddSingleton<HelpCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        services.AddSingleton<VersionCommand>()
                .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        services.AddNode(configuration);
        services.AddExplorer(_builder.Configuration);
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

}
