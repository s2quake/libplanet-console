using System.Collections.Specialized;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Executable.Tracers;

internal sealed class NodeCollectionEventTracer : IHostedService, IDisposable
{
    private readonly INodeCollection _nodes;
    private bool _isDisposed;

    public NodeCollectionEventTracer(INodeCollection nodes)
    {
        _nodes = nodes;
        foreach (var node in _nodes)
        {
            AttachEvent(node);
        }

        _nodes.CollectionChanged += Nodes_CollectionChanged;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    void IDisposable.Dispose()
    {
        if (_isDisposed is false)
        {
            _nodes.CollectionChanged -= Nodes_CollectionChanged;
            foreach (var node in _nodes)
            {
                DetachEvent(node);
            }

            _isDisposed = true;
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
