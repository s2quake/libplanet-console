using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Executable.Tracers;

internal sealed class BlockChainEventTracer(IBlockChain blockChain)
    : IApplicationService, IDisposable
{
    public Task InitializeAsync(CancellationToken cancellationToken)
    {
        blockChain.BlockAppended += Node_BlockAppended;
        return Task.CompletedTask;
    }

    void IDisposable.Dispose()
    {
        blockChain.BlockAppended -= Node_BlockAppended;
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        var message = $"Block #{blockInfo.Height} '{hash.ToShortString()}' " +
                      $"Appended by '{miner.ToShortString()}'";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}
