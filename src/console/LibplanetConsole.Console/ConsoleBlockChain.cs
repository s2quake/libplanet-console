using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

internal sealed class ConsoleBlockChain(NodeCollection nodes)
    : IConsoleContent, IBlockChain
{
    private IBlockChain? _blockChain;
    private Node? _node;

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public BlockInfo Tip { get; private set; } = BlockInfo.Empty;

    public bool IsRunning { get; private set; }

    string IConsoleContent.Name => "blockchain";

    IEnumerable<IConsoleContent> IConsoleContent.Dependencies { get; } = [];

    public Task SendTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.SendTransactionAsync(transaction, cancellationToken);
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

    Task<AddressInfo[]> IBlockChain.GetAddressesAsync(CancellationToken cancellationToken)
    {
        if (IsRunning is false || _blockChain is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _blockChain.GetAddressesAsync(cancellationToken);
    }

    async Task IConsoleContent.StartAsync(CancellationToken cancellationToken)
    {
        SetNode(nodes.Current);
        nodes.CurrentChanged += Nodes_CurrentChanged;
        await Task.CompletedTask;
    }

    async Task IConsoleContent.StopAsync(CancellationToken cancellationToken)
    {
        nodes.CurrentChanged -= Nodes_CurrentChanged;
        SetNode(null);
        await Task.CompletedTask;
    }

    private void SetNode(Node? node)
    {
        if (_blockChain is not null)
        {
            _blockChain.BlockAppended -= BlockChain_BlockAppended;
            _blockChain.Started -= BlockChain_Started;
            _blockChain.Stopped -= BlockChain_Stopped;
            if (_blockChain.IsRunning is true)
            {
                Tip = BlockInfo.Empty;
                IsRunning = false;
                Stopped?.Invoke(this, EventArgs.Empty);
            }
        }

        _node = node;
        _blockChain = node?.GetRequiredKeyedService<IBlockChain>(INode.Key);

        if (_blockChain is not null)
        {
            _blockChain.BlockAppended += BlockChain_BlockAppended;
            _blockChain.Started += BlockChain_Started;
            _blockChain.Stopped += BlockChain_Stopped;
            if (_blockChain.IsRunning is true)
            {
                Tip = _blockChain.Tip;
                IsRunning = true;
                Started?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void Nodes_CurrentChanged(object? sender, EventArgs e) => SetNode(nodes.Current);

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
        => BlockAppended?.Invoke(sender, e);

    private void BlockChain_Started(object? sender, EventArgs e)
    {
        Tip = _blockChain?.Tip ?? BlockInfo.Empty;
        IsRunning = true;
        Started?.Invoke(this, EventArgs.Empty);
    }

    private void BlockChain_Stopped(object? sender, EventArgs e)
    {
        Tip = BlockInfo.Empty;
        IsRunning = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }
}
