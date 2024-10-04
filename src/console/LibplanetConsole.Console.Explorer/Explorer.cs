using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Explorer;
using LibplanetConsole.Explorer.Services;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console.Explorer;

internal sealed class Explorer : NodeContentBase, IExplorer, IExplorerCallback, INodeContentService
{
    private readonly ILogger _logger;
    private readonly ExecutionScope _executionScope = new();
    private readonly ExplorerSettings _settings;
    private EndPoint _endPoint = EndPointUtility.NextEndPoint();
    private RemoteService<IExplorerService, IExplorerCallback>? _remoteService;

    public Explorer(INode node, ILogger<Explorer> logger, ExplorerSettings settings)
        : base(node)
    {
        _settings = settings;
        _logger = logger;
        _logger.LogDebug("Explorer is created: {NodeAddress}", node.Address);
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public EndPoint EndPoint
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

    IRemoteService INodeContentService.RemoteService => RemoteService;

    private IExplorerService Service => RemoteService.Service;

    private RemoteService<IExplorerService, IExplorerCallback> RemoteService
        => _remoteService ??= new RemoteService<IExplorerService, IExplorerCallback>(this);

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
        _logger.LogDebug("Explorer is started: {NodeAddress} {EndPoint}", nodeAddress, endPoint);
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
        _logger.LogDebug("Explorer is stopped.: {NodeAddress}", Node.Address);
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
            _logger.LogDebug(message, nodeAddress, endPoint);
            Started?.Invoke(this, EventArgs.Empty);
        }
    }

    void IExplorerCallback.OnStopped()
    {
        if (_executionScope.IsExecuting != true)
        {
            IsRunning = false;
            _logger.LogDebug("Explorer is stopped by the remote service: {NodeAddress}", Node.Address);
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

        if (_settings.UseExplorer == true)
        {
            await StartAsync(cancellationToken: default);
        }
    }
}
