using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Executable.Tracers;

internal sealed class NodeEventTracer(IApplication application, INode node)
    : IApplicationService, IDisposable
{
    public Task InitializeAsync(CancellationToken cancellationToken)
    {
        node.Started += Node_Started;
        node.Stopped += Node_Stopped;
        return Task.CompletedTask;
    }

    void IDisposable.Dispose()
    {
        node.Started -= Node_Started;
        node.Stopped -= Node_Stopped;
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        var endPoint = application.Info.EndPoint;
        var message = $"BlockChain has been started.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        var endPoint = application.Info.EndPoint;
        var message = $"BlockChain has been stopped.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}
