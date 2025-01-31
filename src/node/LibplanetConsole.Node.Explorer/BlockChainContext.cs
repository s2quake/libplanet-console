using System.Reflection;
using Libplanet.Explorer.Indexing;
using Libplanet.Explorer.Interfaces;
using Libplanet.Net;
using Libplanet.Store;

namespace LibplanetConsole.Node.Explorer;

internal sealed class BlockChainContext(INode node) : IBlockChainContext
{
    public bool Preloaded => false;

    public Libplanet.Blockchain.BlockChain BlockChain
        => node.GetRequiredService<Libplanet.Blockchain.BlockChain>();

#pragma warning disable S3011 // Reflection should not be used to increase accessibility ...
    public IStore Store
    {
        get
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var blockChainType = typeof(Libplanet.Blockchain.BlockChain);
            var propertyInfo = blockChainType.GetProperty("Store", bindingFlags) ??
                throw new InvalidOperationException("Store property not found.");
            if (propertyInfo.GetValue(BlockChain) is IStore store)
            {
                return store;
            }

            throw new InvalidOperationException("Store property is not IStore.");
        }
    }
#pragma warning restore S3011

    public Swarm Swarm => node.GetRequiredService<Swarm>();

    public IBlockChainIndex Index => throw new NotSupportedException();
}
