// using System.Security.Cryptography;

// namespace LibplanetConsole.Node.Services;

// public interface IBlockChainService
// {
//     Task<TxId> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

//     Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);

//     Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken);

//     Task<byte[]> GetStateAsync(
//         BlockHash? blockHash,
//         Address accountAddress,
//         Address address,
//         CancellationToken cancellationToken);

//     Task<byte[]> GetStateByStateRootHashAsync(
//         HashDigest<SHA256> stateRootHash,
//         Address accountAddress,
//         Address address,
//         CancellationToken cancellationToken);

//     Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

//     Task<byte[]> GetActionAsync(TxId txId, int actionIndex, CancellationToken cancellationToken);
// }
