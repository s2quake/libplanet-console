using System.Diagnostics;
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Threading;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Grpc.Blockchain;
using LibplanetConsole.Grpc.Node;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed partial class Node : INode
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NodeOptions _nodeOptions;
    private readonly PrivateKey _privateKey;
    private readonly ILogger _logger;
    private NodeService? _nodeService;
    private BlockChainService? _blockChainService;
    private GrpcChannel? _channel;
    private NodeInfo _info;
    private bool _isDisposed;
    private NodeProcess? _process;
    private CancellationTokenSource? _processCancellationTokenSource;
    private Task _processTask = Task.CompletedTask;
    private INodeContent[]? _contents;
    private string? _commandLine;

    public Node(IServiceProvider serviceProvider, NodeOptions nodeOptions)
    {
        _serviceProvider = serviceProvider;
        _nodeOptions = nodeOptions;
        _privateKey = nodeOptions.PrivateKey;
        _logger = _serviceProvider.GetLogger<Node>();
        PublicKey = nodeOptions.PrivateKey.PublicKey;
        _logger.LogDebug("Node is created: {Address}", Address);
    }

    public event EventHandler? Attached;

    public event EventHandler? Detached;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public event EventHandler? Disposed;

    public PublicKey PublicKey { get; }

    public Address Address => PublicKey.Address;

    public bool IsAttached { get; private set; }

    public bool IsRunning { get; private set; }

    public int ProcessId => _process?.Id ?? -1;

    public EndPoint EndPoint => _nodeOptions.EndPoint;

    public NodeInfo Info => _info;

    public INodeContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (_serviceProvider is IKeyedServiceProvider serviceProvider)
        {
            return serviceProvider.GetKeyedService(serviceType, serviceKey);
        }

        throw new InvalidOperationException("Service provider does not support keyed service.");
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        if (_serviceProvider is IKeyedServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredKeyedService(serviceType, serviceKey);
        }

        throw new InvalidOperationException("Service provider does not support keyed service.");
    }

    public override string ToString() => $"{Address}: {EndPointUtility.ToString(EndPoint)}";

    public byte[] Sign(object obj) => _nodeOptions.PrivateKey.Sign(obj);

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_nodeService is null)
        {
            throw new InvalidOperationException("Node is not attached.");
        }

        var request = new GetInfoRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _nodeService.GetInfoAsync(request, callOptions);
        _info = response.NodeInfo;
        return _info;
    }

    public async Task AttachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_channel is not null)
        {
            throw new InvalidOperationException("Node is already attached.");
        }

        var channel = NodeChannel.CreateChannel(_nodeOptions.EndPoint);
        var nodeService = new NodeService(channel);
        var blockChainService = new BlockChainService(channel);
        nodeService.Started += NodeService_Started;
        nodeService.Stopped += NodeService_Stopped;
        nodeService.Disconnected += NodeService_Disconnected;
        blockChainService.BlockAppended += BlockChainService_BlockAppended;
        try
        {
            await nodeService.InitializeAsync(cancellationToken);
            await blockChainService.InitializeAsync(cancellationToken);
        }
        catch
        {
            nodeService.Dispose();
            blockChainService.Dispose();
            throw;
        }

        _channel = channel;
        _nodeService = nodeService;
        _blockChainService = blockChainService;
        _info = await GetInfoAsync(cancellationToken);
        IsRunning = _info.IsRunning;
        IsAttached = true;
        _logger.LogDebug("Node is attached: {Address}", Address);
        Attached?.Invoke(this, EventArgs.Empty);
    }

    public async Task DetachAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_channel is null)
        {
            throw new InvalidOperationException("Node is not attached.");
        }

        if (_nodeService is not null)
        {
            _nodeService.Started -= NodeService_Started;
            _nodeService.Stopped -= NodeService_Stopped;
            _nodeService.Disconnected -= NodeService_Disconnected;
            _nodeService.Dispose();
            _nodeService = null;
        }

        if (_blockChainService is not null)
        {
            _blockChainService.BlockAppended -= BlockChainService_BlockAppended;
            _blockChainService.Dispose();
            _blockChainService = null;
        }

        await _channel.ShutdownAsync();
        _channel.Dispose();
        _channel = null;
        IsRunning = false;
        IsAttached = false;
        _logger.LogDebug("Node is detached: {Address}", Address);
        Detached?.Invoke(this, EventArgs.Empty);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (IsRunning is true)
        {
            throw new InvalidOperationException("Node is already running.");
        }

        if (_nodeService is null)
        {
            throw new InvalidOperationException("Node is not attached.");
        }

        var seedEndPoint = GetSeedEndPoint();
        var request = new StartRequest
        {
            SeedEndPoint = EndPointUtility.ToString(seedEndPoint),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _nodeService.StartAsync(request, callOptions);
        _info = response.NodeInfo;
        IsRunning = true;
        _logger.LogDebug("Node is started: {Address}", Address);
        await Task.WhenAll(Contents.Select(item => item.StartAsync(cancellationToken)));
        _logger.LogDebug("Node Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        if (_nodeService is null)
        {
            throw new InvalidOperationException("Node is not attached.");
        }

        await Task.WhenAll(Contents.Select(item => item.StopAsync(cancellationToken)));
        _logger.LogDebug("Node Contents are stopped: {Address}", Address);

        var request = new StopRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _nodeService.StopAsync(request, callOptions);
        _info = NodeInfo.Empty;
        IsRunning = false;
        _logger.LogDebug("Node is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed is false)
        {
            if (_processCancellationTokenSource is not null)
            {
                await _processCancellationTokenSource.CancelAsync();
                _processCancellationTokenSource.Dispose();
                _processCancellationTokenSource = null;
            }

            await TaskUtility.TryWait(_processTask);
            _processTask = Task.CompletedTask;
            _process = null;

            if (_nodeService is not null)
            {
                _nodeService.Started -= NodeService_Started;
                _nodeService.Stopped -= NodeService_Stopped;
                _nodeService.Disconnected -= NodeService_Disconnected;
                _nodeService.Dispose();
                _nodeService = null;
            }

            if (_blockChainService is not null)
            {
                _blockChainService.BlockAppended -= BlockChainService_BlockAppended;
                _blockChainService.Dispose();
                _blockChainService = null;
            }

            IsRunning = false;
            _isDisposed = true;
            _logger.LogDebug("Node is disposed: {Address}", Address);
            Disposed?.Invoke(this, EventArgs.Empty);
            GC.SuppressFinalize(this);
        }
    }

    public async Task StartProcessAsync(ProcessOptions options, CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("Node process is already running.");
        }

        var process = CreateProcess(options);
        var processCancellationTokenSource = new CancellationTokenSource();
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, processCancellationTokenSource.Token);

        _logger.LogDebug("Commands: {CommandLine}", process.ToString());
        _processTask = process.RunAsync(cancellationTokenSource.Token)
            .ContinueWith(
                task =>
                {
                    _processTask = Task.CompletedTask;
                    _process = null;
                    _processCancellationTokenSource?.Dispose();
                    _processCancellationTokenSource = null;
                    cancellationTokenSource.Dispose();
                },
                cancellationToken);
        _process = process;
        _processCancellationTokenSource = processCancellationTokenSource;

        if (options.Detach is false)
        {
            await AttachAsync(cancellationToken);
        }

        if (IsAttached is true && _nodeOptions.SeedEndPoint is null)
        {
            await StartAsync(cancellationToken);
        }
    }

    public async Task StopProcessAsync(CancellationToken cancellationToken)
    {
        if (_process is null)
        {
            throw new InvalidOperationException("Node process is not running.");
        }

        if (_processCancellationTokenSource is not null)
        {
            await _processCancellationTokenSource.CancelAsync();
            _processCancellationTokenSource.Dispose();
            _processCancellationTokenSource = null;
        }

        await TaskUtility.TryWait(_processTask);
        _processTask = Task.CompletedTask;
        _process = null;
    }

    public string GetCommandLine()
    {
        if (_commandLine is null)
        {
            var process = CreateProcess(ProcessOptions.Default);
            _commandLine = process.GetCommandLine();
        }

        return _commandLine ?? throw new UnreachableException("Process is not created.");
    }

    private void NodeService_Started(object? sender, NodeEventArgs e)
    {
        _info = e.NodeInfo;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    private void NodeService_Stopped(object? sender, EventArgs e)
    {
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    private void BlockChainService_BlockAppended(object? sender, BlockEventArgs e)
    {
        _info = _info with { Tip = e.BlockInfo };
        BlockAppended?.Invoke(this, e);
    }

    private void NodeService_Disconnected(object? sender, EventArgs e)
    {
        if (sender is NodeService nodeService && _nodeService == nodeService)
        {
            _nodeService.Started -= NodeService_Started;
            _nodeService.Stopped -= NodeService_Stopped;
            _nodeService.Disconnected -= NodeService_Disconnected;
            _nodeService.Dispose();
            _nodeService = null;
            if (_blockChainService is not null)
            {
                _blockChainService.BlockAppended -= BlockChainService_BlockAppended;
                _blockChainService.Dispose();
                _blockChainService = null;
            }

            if (_channel is not null)
            {
                _channel.Dispose();
                _channel = null;
            }

            if (IsRunning is true)
            {
                IsRunning = false;
                Stopped?.Invoke(this, EventArgs.Empty);
            }

            IsAttached = false;
            Detached?.Invoke(this, EventArgs.Empty);
        }
    }

    private EndPoint GetSeedEndPoint()
    {
        if (_nodeOptions.SeedEndPoint is { } seedEndPoint)
        {
            return seedEndPoint;
        }

        var server = _serviceProvider.GetRequiredService<IServer>();
        var addressesFeature = server.Features.Get<IServerAddressesFeature>()
            ?? throw new InvalidOperationException("ServerAddressesFeature is not available.");
        var address = addressesFeature.Addresses.First();
        var url = new Uri(address);

        return new DnsEndPoint(url.Host, url.Port);
    }

    private NodeProcess CreateProcess(ProcessOptions options)
    {
        var nodeOptions = _nodeOptions;
        var process = new NodeProcess(nodeOptions)
        {
            Detach = options.Detach,
            NewWindow = options.NewWindow,
        };

        if (nodeOptions.RepositoryPath == string.Empty)
        {
            var applicationOptions = _serviceProvider.GetRequiredService<IApplicationOptions>();
            process.ExtendedArguments.Add("--genesis");
            process.ExtendedArguments.Add(BlockUtility.ToString(applicationOptions.GenesisBlock));
            process.ExtendedArguments.Add("--apv");
            process.ExtendedArguments.Add(applicationOptions.AppProtocolVersion.Token);

            if (applicationOptions.LogPath != string.Empty)
            {
                var nodeLogPath = Path.Combine(
                    applicationOptions.LogPath, "nodes", Address.ToString());
                process.ExtendedArguments.Add("--log-path");
                process.ExtendedArguments.Add(nodeLogPath);
            }
        }

        return process;
    }
}
