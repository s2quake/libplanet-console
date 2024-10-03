using Libplanet.Explorer;
using LibplanetConsole.Common;
using LibplanetConsole.Explorer;
using LibplanetConsole.Framework;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Node.Explorer;

[Export(typeof(IExplorer))]
[Export(typeof(IApplicationService))]
[Export]
internal sealed class Explorer(
    INode node, ILogger logger, ExplorerSettings settings) : IExplorer, IApplicationService
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

        var (host, port) = EndPointUtility.GetHostAndPort(options.EndPoint);
        _webHost = WebHost.CreateDefaultBuilder()
                    .ConfigureServices(services => services.AddSingleton(node))
                    .UseStartup<ExplorerStartup<BlockChainContext>>()
                    .UseSerilog()
                    .UseUrls($"http://{host}:{port}/")
                    .Build();

        await _webHost.StartAsync(cancellationToken);
        Info = new()
        {
            EndPoint = options.EndPoint,
            IsRunning = true,
            Url = $"http://{host}:{port}/ui/playground",
        };
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

    async Task IApplicationService.InitializeAsync(CancellationToken cancellationToken)
    {
        if (settings.IsExplorerEnabled is true)
        {
            var options = new ExplorerOptions
            {
                EndPoint = EndPointUtility.ParseOrNext(settings.ExplorerEndPoint),
            };
            await StartAsync(options, cancellationToken);
        }
    }
}
