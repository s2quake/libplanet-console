using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Explorer;
using LibplanetConsole.Explorer.Services;
using LibplanetConsole.Frameworks;
using Serilog;

namespace LibplanetConsole.Consoles.Explorer;

[Export(typeof(IExplorerNodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(INodeContent))]
internal sealed class ExplorerNodeContent
    : NodeContentBase, IExplorerNodeContent, IExplorerCallback, INodeContentService
{
    private readonly RemoteService<IExplorerService, IExplorerCallback> _remoteService;
    private readonly ILogger _logger;
    private readonly ExecutionScope _executionScope = new();
    private AppEndPoint _endPoint = AppEndPoint.Next();

    [ImportingConstructor]
    public ExplorerNodeContent(INode node, ILogger logger)
        : base(node)
    {
        _remoteService = new RemoteService<IExplorerService, IExplorerCallback>(this);
        _logger = logger;
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public AppEndPoint EndPoint
    {
        get => _endPoint;
        set
        {
            if (IsRunning == true)
            {
                throw new InvalidOperationException(
                    "Cannot change the endpoint while the explorer is running.");
            }

            _endPoint = value;
        }
    }

    public ExplorerInfo Info { get; private set; }

    public bool IsRunning { get; private set; }

    IRemoteService INodeContentService.RemoteService => _remoteService;

    private IExplorerService Service => _remoteService.Service;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == true)
        {
            throw new InvalidOperationException("The explorer is already running.");
        }

        using var scope = _executionScope.Enter();
        var nodeAddress = Node.Address;
        var endPoint = EndPoint.ToString();
        var options = new ExplorerOptions
        {
            EndPoint = EndPoint,
        };
        Info = await Service.StartAsync(options, cancellationToken);
        IsRunning = true;
        _logger.Debug("Explorer is started: {NodeAddress} {EndPoint}", nodeAddress, endPoint);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsRunning != true)
        {
            throw new InvalidOperationException("The explorer is not running.");
        }

        using var scope = _executionScope.Enter();
        await Service.StopAsync(cancellationToken);
        IsRunning = false;
        _logger.Debug("Explorer is stopped.: {NodeAddress}", Node.Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    void IExplorerCallback.OnStarted(ExplorerInfo explorerInfo)
    {
        if (_executionScope.IsExecuting != true)
        {
            var message = "Explorer is started by the remote service: {NodeAddress} {EndPoint}";
            var nodeAddress = Node.Address;
            var endPoint = EndPoint.ToString();
            Info = explorerInfo;
            IsRunning = true;
            _logger.Debug(message, nodeAddress, endPoint);
            Started?.Invoke(this, EventArgs.Empty);
        }
    }

    void IExplorerCallback.OnStopped()
    {
        if (_executionScope.IsExecuting != true)
        {
            IsRunning = false;
            _logger.Debug("Explorer is stopped by the remote service: {NodeAddress}", Node.Address);
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

    protected async override void OnNodeAttached()
    {
        base.OnNodeAttached();
        Info = await Service.GetInfoAsync(cancellationToken: default);
        IsRunning = Info.IsRunning;
    }

    protected async override void OnNodeStarted()
    {
        base.OnNodeStarted();

        var settings = ApplicationSettingsParser.Peek<ExplorerNodeSettings>();
        if (settings.UseExplorer == true)
        {
            await StartAsync(cancellationToken: default);
        }
    }
}
