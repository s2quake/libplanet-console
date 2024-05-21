using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Banks.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class BankNodeService(BankNode bankNode, INode node)
    : LocalService<IBankService>, IBankService
{
    public async Task<BalanceInfo> MintAsync(
        byte[] signature, double amount, CancellationToken cancellationToken)
    {
        if (node.Verify(amount, signature) == true)
        {
            return await bankNode.MintAsync(amount, cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signature));
    }

    public Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken)
        => bankNode.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => bankNode.GetPoolAsync(cancellationToken);
}
