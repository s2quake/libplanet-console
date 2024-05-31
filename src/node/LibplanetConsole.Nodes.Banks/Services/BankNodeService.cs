using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes.Banks.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class BankNodeService(BankNode bankNode, INode node)
    : LocalService<IBankService>, IBankService
{
    public Task<BalanceInfo> MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        mintOptions.Verify(node);
        return bankNode.MintAsync(mintOptions.Amount, cancellationToken);
    }

    public Task<BalanceInfo> TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        transferOptions.Verify(node);
        return bankNode.TransferAsync(
            targetAddress: transferOptions.TargetAddress,
            amount: transferOptions.Amount,
            cancellationToken: cancellationToken);
    }

    public Task<BalanceInfo> BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        burnOptions.Verify(node);
        return bankNode.BurnAsync(burnOptions.Amount, cancellationToken);
    }

    public Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken)
        => bankNode.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => bankNode.GetPoolAsync(cancellationToken);
}
