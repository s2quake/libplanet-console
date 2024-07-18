using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Clients;

internal sealed partial class Client : IBlockChain
{
    public async Task<AppId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var privateKey = AppPrivateKey.FromSecureString(_privateKey);
        var address = privateKey.Address;
        var nonce = await RemoteBlockChainService.GetNextNonceAsync(address, cancellationToken);
        var genesisHash = NodeInfo.GenesisHash;
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: (PrivateKey)privateKey,
            genesisHash: (BlockHash)genesisHash,
            actions: [.. actions.Select(item => item.PlainValue)]);
        var txData = tx.Serialize();
        _logger.Debug("Client sends a transaction: {AppId}", tx.Id);
        return await RemoteBlockChainService.SendTransactionAsync(txData, cancellationToken);
    }

    public Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
        => RemoteBlockChainService.GetBlockHashAsync(height, cancellationToken);

    public Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken)
        => RemoteBlockChainService.GetNextNonceAsync(address, cancellationToken);

    public Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken)
        => RemoteBlockChainService.GetTipHashAsync(cancellationToken);

    public Task<byte[]> GetStateByBlockHashAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
        => RemoteBlockChainService.GetStateByBlockHashAsync(
            blockHash, accountAddress, address, cancellationToken);

    public Task<byte[]> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
        => RemoteBlockChainService.GetStateByStateRootHashAsync(
            stateRootHash, accountAddress, address, cancellationToken);
}
