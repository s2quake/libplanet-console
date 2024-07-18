using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class NodeEventTracer(IApplication application, INode node) : IApplicationService
{
    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        node.BlockAppended += Node_BlockAppended;
        node.Started += Node_Started;
        node.Stopped += Node_Stopped;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        node.BlockAppended -= Node_BlockAppended;
        node.Started -= Node_Started;
        node.Stopped -= Node_Stopped;
        return ValueTask.CompletedTask;
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        var message = $"Block #{blockInfo.Index} '{hash:S}' Appended by '{miner:S}'";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
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
