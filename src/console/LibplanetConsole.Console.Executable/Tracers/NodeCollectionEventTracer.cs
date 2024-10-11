using System.Collections.Specialized;
using JSSoft.Terminals;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Executable.Tracers;

internal sealed class NodeCollectionEventTracer : IHostedService, IDisposable
{
    private readonly INodeCollection _nodes;
    private INode? _current;

    public NodeCollectionEventTracer(INodeCollection nodes)
    {
        _nodes = nodes;
        UpdateCurrent(_nodes.Current);
        foreach (var node in _nodes)
        {
            AttachEvent(node);
        }

        _nodes.CurrentChanged += Nodes_CurrentChanged;
        _nodes.CollectionChanged += Nodes_CollectionChanged;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    void IDisposable.Dispose()
    {
        _nodes.CurrentChanged -= Nodes_CurrentChanged;
        _nodes.CollectionChanged -= Nodes_CollectionChanged;
        UpdateCurrent(null);
        foreach (var node in _nodes)
        {
            DetachEvent(node);
        }
    }

    private void UpdateCurrent(INode? node)
    {
        if (_current?.GetService(typeof(IBlockChain)) is IBlockChain blockChain1)
        {
            blockChain1.BlockAppended -= BlockChain_BlockAppended;
        }

        _current = node;

        if (_current?.GetService(typeof(IBlockChain)) is IBlockChain blockChain2)
        {
            blockChain2.BlockAppended += BlockChain_BlockAppended;
        }
    }

    private void AttachEvent(INode node)
    {
        node.Attached += Node_Attached;
        node.Detached += Node_Detached;
        node.Started += Node_Started;
        node.Stopped += Node_Stopped;
    }

    private void DetachEvent(INode node)
    {
        node.Attached -= Node_Attached;
        node.Detached -= Node_Detached;
        node.Started -= Node_Started;
        node.Stopped -= Node_Stopped;
    }

    private void Nodes_CurrentChanged(object? sender, EventArgs e)
    {
        UpdateCurrent(_nodes.Current);
    }

    private void Nodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (INode node in e.NewItems!)
            {
                var message = $"Node created: {node.Address.ToShortString()}";
                var colorType = TerminalColorType.BrightBlue;
                System.Console.Out.WriteColoredLine(message, colorType);
                AttachEvent(node);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (INode node in e.OldItems!)
            {
                var message = $"Node deleted: {node.Address.ToShortString()}";
                var colorType = TerminalColorType.BrightBlue;
                System.Console.Out.WriteColoredLine(message, colorType);
                DetachEvent(node);
            }
        }
    }

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        var message = $"Block #{blockInfo.Height} '{hash.ToShortString()}' " +
                      $"Appended by '{miner.ToShortString()}'";
        System.Console.Out.WriteColoredLine(message, TerminalColorType.BrightBlue);
    }

    private void Node_Attached(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node attached: {node.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Detached(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node detached: {node.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node started: {node.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node stopped: {node.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }
}
