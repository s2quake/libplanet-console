using System.Collections.Specialized;
using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Executable.Tracers;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class NodeCollectionEventTracer(INodeCollection nodes) : IApplicationService
{
    private readonly INodeCollection _nodes = nodes;
    private INode? _current;

    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        UpdateCurrent(_nodes.Current);
        foreach (var node in _nodes)
        {
            AttachEvent(node);
        }

        _nodes.CurrentChanged += Nodes_CurrentChanged;
        _nodes.CollectionChanged += Nodes_CollectionChanged;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        UpdateCurrent(null);
        foreach (var node in _nodes)
        {
            DetachEvent(node);
        }

        _nodes.CurrentChanged -= Nodes_CurrentChanged;
        _nodes.CollectionChanged -= Nodes_CollectionChanged;
        return ValueTask.CompletedTask;
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
                var message = $"Node created: {node.Address:S}";
                var colorType = TerminalColorType.BrightBlue;
                Console.Out.WriteColoredLine(message, colorType);
                AttachEvent(node);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (INode node in e.OldItems!)
            {
                var message = $"Node deleted: {node.Address:S}";
                var colorType = TerminalColorType.BrightBlue;
                Console.Out.WriteColoredLine(message, colorType);
                DetachEvent(node);
            }
        }
    }

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        var message = $"Block #{blockInfo.Height} '{hash:S}' Appended by '{miner:S}'";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightBlue);
    }

    private void Node_Attached(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node attached: {node.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Detached(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node detached: {node.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node started: {node.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node stopped: {node.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }
}
