using System.Security.Cryptography;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// namespace LibplanetConsole.Node.Services;

// internal sealed class BlockChainService
//     : LocalService<IBlockChainService, IBlockChainCallback>, IBlockChainService
// {
//     private static readonly Codec _codec = new();
//     private readonly ILogger _logger;
//     private readonly Node _node;

//    public BlockChainService(Node node)
//    {
//        _node = node;
//        _logger = node.GetLogger<BlockChainService>();
//        _node.BlockAppended += Node_BlockAppended;
//    }

//     public async Task<TxId> SendTransactionAsync(
//         byte[] transaction, CancellationToken cancellationToken)
//     {
//         var tx = Transaction.Deserialize(transaction);
//         await _node.AddTransactionAsync(tx, cancellationToken);
//         return tx.Id;
//     }

//     public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
//         => _node.GetNextNonceAsync(address, cancellationToken);

//     public Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken)
//         => _node.GetTipHashAsync(cancellationToken);

//     public async Task<byte[]> GetStateAsync(
//         BlockHash? blockHash,
//         Address accountAddress,
//         Address address,
//         CancellationToken cancellationToken)
//     {
//         var value = await _node.GetStateAsync(
//             blockHash, accountAddress, address, cancellationToken);
//         return _codec.Encode(value);
//     }

//     public async Task<byte[]> GetStateByStateRootHashAsync(
//         HashDigest<SHA256> stateRootHash,
//         Address accountAddress,
//         Address address,
//         CancellationToken cancellationToken)
//     {
//         var value = await _node.GetStateByStateRootHashAsync(
//             stateRootHash, accountAddress, address, cancellationToken);
//         return _codec.Encode(value);
//     }

//     public Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
//         => _node.GetBlockHashAsync(height, cancellationToken);

//     public Task<byte[]> GetActionAsync(
//         TxId txId, int actionIndex, CancellationToken cancellationToken)
//         => _node.GetActionAsync(txId, actionIndex, cancellationToken);

//    private void Node_BlockAppended(object? sender, BlockEventArgs e)
//    {
//        var blockInfo = e.BlockInfo;
//        Callback.OnBlockAppended(blockInfo);
//        _logger.LogDebug("Callback.OnBlockAppended: {BlockHash}", blockInfo.Hash);
//    }
//}
