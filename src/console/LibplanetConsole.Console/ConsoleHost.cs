using System.Collections.Specialized;
using LibplanetConsole.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed partial class ConsoleHost : IConsole, IDisposable
{
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly PrivateKey _privateKey;
    private readonly BlockHash _genesisHash;
    private readonly ILogger<ConsoleHost> _logger;
    private Node? _node;
    private IConsoleContent[]? _contents;
    private bool _isDisposed;
    private CancellationTokenSource? _cancellationTokenSource;

    public ConsoleHost(
        NodeCollection nodes,
        ClientCollection clients,
        IApplicationOptions options,
        ILogger<ConsoleHost> logger)
    {
        _nodes = nodes;
        _clients = clients;
        _privateKey = options.PrivateKey;
        _genesisHash = options.GenesisBlock.Hash;
        _logger = logger;
        _nodes.CollectionChanged += Nodes_CollectionChanged;
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public BlockInfo Tip { get; private set; } = BlockInfo.Empty;

    public bool IsRunning { get; private set; }

    public Address Address => _privateKey.Address;

    public IConsoleContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is true)
        {
            throw new InvalidOperationException("Node is already running.");
        }

        _cancellationTokenSource = new();
        IsRunning = true;
        _logger.LogDebug("Console is started: {Address}", Address);
        await Task.WhenAll(Contents.Select(item => item.StartAsync(cancellationToken)));
        _logger.LogDebug("Console Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        await Task.WhenAll(Contents.Select(item => item.StopAsync(cancellationToken)));
        _logger.LogDebug("Console Contents are stopped: {Address}", Address);

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        IsRunning = false;
        _logger.LogDebug("Console is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task InitializeAsync()
    {
        _cancellationTokenSource = new();
        try
        {
            await _nodes.InitializeAsync(_cancellationTokenSource.Token);
            await _clients.InitializeAsync(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException e)
        {
            _logger.LogDebug(e, "The console was canceled.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while starting the console.");
        }
    }

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        var privateKey = _privateKey;
        var genesisHash = _genesisHash;
        var nonce = await _node.GetNextNonceAsync(privateKey.Address, cancellationToken);
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisHash,
            actions: new TxActionList(values));

        await _node.SendTransactionAsync(transaction, cancellationToken);
        return transaction.Id;
    }

    void IDisposable.Dispose()
    {
        if (_isDisposed is false)
        {
            _nodes.CollectionChanged -= Nodes_CollectionChanged;
            _isDisposed = true;
        }
    }

    private void Nodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    foreach (Node node in e.NewItems)
                    {
                        node.BlockAppended += Node_BlockAppended;
                        node.Started += Node_Started;
                        node.Stopped += Node_Stopped;
                    }
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is not null)
                {
                    foreach (Node node in e.OldItems)
                    {
                        node.BlockAppended -= Node_BlockAppended;
                        node.Started -= Node_Started;
                        node.Stopped -= Node_Stopped;
                    }
                }

                break;
        }
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        if (e.BlockInfo.Height > Tip.Height)
        {
            Tip = e.BlockInfo;
            BlockAppended?.Invoke(sender, e);
        }
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        _node = GetRandomNode();
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        _node = GetRandomNode();
    }

    private Node? GetRandomNode()
    {
        var nodeList = new List<Node>(_nodes.Count);
        for (var i = 0; i < _nodes.Count; i++)
        {
            var node = _nodes[i];
            if (node.IsRunning is true)
            {
                nodeList.Add(node);
            }
        }

        if (nodeList.Count is 0)
        {
            return null;
        }

        var index = Random.Shared.Next(nodeList.Count);
        return nodeList[index];
    }
}
