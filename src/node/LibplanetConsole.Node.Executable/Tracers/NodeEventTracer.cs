using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Executable.Tracers;

internal sealed class NodeEventTracer : IHostedService, IDisposable
{
    private readonly ApplicationOptions _options;
    private readonly INode _node;

    public NodeEventTracer(ApplicationOptions options, INode node)
    {
        _options = options;
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
        var endPoint = _options.Port;
        var message = $"Node has been started.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        var endPoint = _options.Port;
        var message = $"Node has been stopped.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}
