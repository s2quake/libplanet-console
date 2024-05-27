using System.Collections.Specialized;
using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Consoles.Executable;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class NodeCollectionEventTracer(INodeCollection nodes) : IApplicationService
{
    private readonly INodeCollection _nodes = nodes;
    private INode? _current;

    public static TextWriter Writer => Console.Out;

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
        if (_current is not null)
        {
            _current.BlockAppended -= Node_BlockAppended;
        }

        _current = node;

        if (_current is not null)
        {
            _current.BlockAppended += Node_BlockAppended;
        }
    }

    private void AttachEvent(INode node)
    {
        node.Started += Node_Started;
        node.Stopped += Node_Stopped;
    }

    private void DetachEvent(INode node)
    {
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
                var message = $"Node attached: {(ShortAddress)node.Address}";
                var colorType = TerminalColorType.BrightBlue;
                Writer.WriteColoredLine(message, colorType);
                node.Started += Node_Started;
                node.Stopped += Node_Stopped;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (INode node in e.OldItems!)
            {
                var message = $"Node detached: {(ShortAddress)node.Address}";
                var colorType = TerminalColorType.BrightBlue;
                Writer.WriteColoredLine(message, colorType);
                node.Started -= Node_Started;
                node.Stopped -= Node_Stopped;
            }
        }
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = (ShortBlockHash)blockInfo.Hash;
        var miner = (ShortAddress)blockInfo.Miner;
        var message = $"Block #{blockInfo.Index} '{hash}' Appended by '{miner}'";
        Writer.WriteColoredLine(message, TerminalColorType.BrightBlue);
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node started: {(ShortAddress)node.Address}";
            var colorType = TerminalColorType.BrightBlue;
            Writer.WriteColoredLine(message, colorType);
        }
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            var message = $"Node stopped: {(ShortAddress)node.Address}";
            var colorType = TerminalColorType.BrightBlue;
            Writer.WriteColoredLine(message, colorType);
        }
    }
}
