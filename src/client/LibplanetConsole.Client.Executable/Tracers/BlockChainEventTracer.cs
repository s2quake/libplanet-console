using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Executable.Tracers;

internal sealed class BlockChainEventTracer : IHostedService, IDisposable
{
    private readonly IBlockChain _blockChain;
    private readonly ILogger<BlockChainEventTracer> _logger;

    public BlockChainEventTracer(
        IBlockChain blockChain, ILogger<BlockChainEventTracer> logger)
    {
        _blockChain = blockChain;
        _logger = logger;
        _blockChain.BlockAppended += Client_BlockAppended;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    void IDisposable.Dispose()
    {
        _blockChain.BlockAppended -= Client_BlockAppended;
    }

    private void Client_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        _logger.LogInformation(
            "Block #{TipHeight} {TipHash} Appended by {TipMiner}",
            blockInfo.Height,
            hash.ToShortString(),
            miner.ToShortString());
    }
}
