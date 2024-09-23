using Bencodex;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Clients;

internal sealed partial class Client : IBlockChain
{
    private static readonly Codec _codec = new();

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

    public async Task<IValue> GetStateAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
    {
        var value = await RemoteBlockChainService.GetStateAsync(
            blockHash, accountAddress, address, cancellationToken);
        return _codec.Decode(value);
    }

    public async Task<IValue> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
    {
        var value = await RemoteBlockChainService.GetStateByStateRootHashAsync(
            stateRootHash, accountAddress, address, cancellationToken);
        return _codec.Decode(value);
    }

    public async Task<T> GetActionAsync<T>(
        AppId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction
    {
        var bytes = await RemoteBlockChainService.GetActionAsync(
            txId, actionIndex, cancellationToken);
        var value = _codec.Decode(bytes);
        if (Activator.CreateInstance(typeof(T)) is T action)
        {
            action.LoadPlainValue(value);
            return action;
        }

        throw new InvalidOperationException("Action not found.");
    }
}
