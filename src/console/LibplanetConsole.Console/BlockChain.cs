using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

internal sealed class BlockChain : IBlockChain, IDisposable
{
    private readonly INodeCollection _nodes;
    private readonly ILogger<BlockChain> _logger;
    private IBlockChain? _blockChain;
    private bool _isDisposed;

    public BlockChain(NodeCollection nodes, ILogger<BlockChain> logger)
    {
        _nodes = nodes;
        _logger = logger;
        UpdateCurrent(_nodes.Current);
        _nodes.CurrentChanged += Nodes_CurrentChanged;
    }

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public BlockInfo Tip { get; private set; } = BlockInfo.Empty;

    public bool IsRunning { get; private set; }

    void IDisposable.Dispose()
    {
        if (_isDisposed is false)
        {
            _nodes.CurrentChanged -= Nodes_CurrentChanged;
            UpdateCurrent(null);

            _isDisposed = true;
        }
    }

    Task<TxId> IBlockChain.SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.SendTransactionAsync(actions, cancellationToken);
    }

    Task<long> IBlockChain.GetNextNonceAsync(
        Address address, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetNextNonceAsync(address, cancellationToken);
    }

    Task<IValue> IBlockChain.GetStateAsync(
        long height,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetStateAsync(height, accountAddress, address, cancellationToken);
    }

    Task<IValue> IBlockChain.GetStateAsync(
        BlockHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetStateAsync(blockHash, accountAddress, address, cancellationToken);
    }

    Task<IValue> IBlockChain.GetStateAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetStateAsync(
            stateRootHash, accountAddress, address, cancellationToken);
    }

    Task<BlockHash> IBlockChain.GetBlockHashAsync(
        long height, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetBlockHashAsync(height, cancellationToken);
    }

    Task<T> IBlockChain.GetActionAsync<T>(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetActionAsync<T>(txId, actionIndex, cancellationToken);
    }

    private void UpdateCurrent(INode? node)
    {
        if (_blockChain is not null)
        {
            _blockChain.Started -= BlockChain_Started;
            _blockChain.Stopped -= BlockChain_Stopped;
            _blockChain.BlockAppended -= BlockChain_BlockAppended;
            if (_blockChain.IsRunning is false)
            {
                Tip = BlockInfo.Empty;
                IsRunning = false;
                _logger.LogDebug("BlockChain is stopped.");
                Stopped?.Invoke(this, EventArgs.Empty);
            }
        }

        _blockChain = node?.GetKeyedService<IBlockChain>(INode.Key);

        if (_blockChain is not null)
        {
            if (_blockChain.IsRunning is true)
            {
                Tip = _blockChain.Tip;
                IsRunning = true;
                _logger.LogDebug("BlockChain is started.");
                Started?.Invoke(this, EventArgs.Empty);
            }

            _blockChain.Started += BlockChain_Started;
            _blockChain.Stopped += BlockChain_Stopped;
            _blockChain.BlockAppended += BlockChain_BlockAppended;
        }
    }

    private void Nodes_CurrentChanged(object? sender, EventArgs e)
        => UpdateCurrent(_nodes.Current);

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
    {
        Tip = e.BlockInfo;
        BlockAppended?.Invoke(sender, e);
    }

    private void BlockChain_Started(object? sender, EventArgs e)
    {
        if (sender is IBlockChain blockChain && blockChain == _blockChain)
        {
            Tip = _blockChain.Tip;
            IsRunning = true;
            _logger.LogDebug("BlockChain is started.");
            Started?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            throw new UnreachableException("The sender is not an instance of IBlockChain.");
        }
    }

    private void BlockChain_Stopped(object? sender, EventArgs e)
    {
        Tip = BlockInfo.Empty;
        IsRunning = false;
        _logger.LogDebug("BlockChain is stopped.");
        Stopped?.Invoke(this, EventArgs.Empty);
    }
}
