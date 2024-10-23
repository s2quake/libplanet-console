using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable.Tracers;

internal sealed class NodeEventTracer : IHostedService, IDisposable
{
    private readonly ApplicationOptions _options;
    private readonly INode _node;

    public NodeEventTracer(INode node, IOptions<ApplicationOptions> options)
    {
        _options = options.Value;
        _node = node;
        _node.Started += Node_Started;
        _node.Stopped += Node_Stopped;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    void IDisposable.Dispose()
    {
        _node.Started -= Node_Started;
        _node.Stopped -= Node_Stopped;
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        var message = $"Node has been started.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        var message = $"Node has been stopped.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}
