using System.ComponentModel.Composition;
using Libplanet.Explorer;
using LibplanetConsole.Common;
using LibplanetConsole.Explorer;
using LibplanetConsole.Frameworks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Nodes.Explorer;

[Export(typeof(IExplorerNode))]
[Export(typeof(IApplicationService))]
[Export]
[method: ImportingConstructor]
internal sealed class ExplorerNode(
    INode node, ILogger logger, ExplorerNodeSettings settings) : IExplorerNode, IApplicationService
{
    private IWebHost? _webHost;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public ExplorerInfo Info { get; private set; }

    public bool IsRunning => _webHost is not null;

    public async Task<ExplorerInfo> StartAsync(
        ExplorerOptions options, CancellationToken cancellationToken)
    {
        if (_webHost is not null)
        {
            throw new InvalidOperationException("The explorer is already running.");
        }

        var endPoint = options.EndPoint;
        _webHost = WebHost.CreateDefaultBuilder()
                    .ConfigureServices(services => services.AddSingleton(node))
                    .UseStartup<ExplorerStartup<BlockChainContext>>()
                    .UseSerilog()
                    .UseUrls($"http://{endPoint.Host}:{endPoint.Port}/")
                    .Build();

        await _webHost.StartAsync(cancellationToken);
        Info = new() { EndPoint = options.EndPoint, IsRunning = true, };
        logger.Debug("Explorer is started: {EndPoint}", Info.EndPoint);
        Started?.Invoke(this, EventArgs.Empty);
        return Info;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_webHost is null)
        {
            throw new InvalidOperationException("The explorer is not running.");
        }

        await _webHost.StopAsync(cancellationToken);
        _webHost = null;
        Info = new() { };
        logger.Debug("Explorer is stopped.");
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var endPoint = settings.ExplorerEndPoint;
        if (endPoint is not null)
        {
            var options = new ExplorerOptions
            {
                EndPoint = AppEndPoint.ParseOrNext(endPoint),
            };
            await StartAsync(options, cancellationToken);
        }
    }
}
