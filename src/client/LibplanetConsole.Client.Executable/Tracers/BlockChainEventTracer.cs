using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Client.Executable.Tracers;

[Export(typeof(IApplicationService))]
internal sealed class BlockChainEventTracer(IBlockChain blockChain)
    : IApplicationService, IDisposable
{
    public Task InitializeAsync(CancellationToken cancellationToken)
    {
        blockChain.BlockAppended += BlockChain_BlockAppended;
        return Task.CompletedTask;
    }

    void IDisposable.Dispose()
    {
        blockChain.BlockAppended -= BlockChain_BlockAppended;
    }

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        var message = $"Block #{blockInfo.Height} '{hash.ToShortString()}' " +
                      $"Appended by '{miner.ToShortString()}'";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}
