using System.Reflection;
using Libplanet.Blockchain;
using Libplanet.Explorer.Indexing;
using Libplanet.Explorer.Interfaces;
using Libplanet.Net;
using Libplanet.Store;

namespace LibplanetConsole.Nodes.Explorer;

internal sealed record class BlockChainContext : IBlockChainContext
{
    private readonly INode _node;

    public BlockChainContext(INode node) => _node = node;

    public bool Preloaded => false;

    public BlockChain BlockChain => _node!.BlockChain;

    public IStore Store
    {
        get
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var propertyInfo = typeof(BlockChain).GetProperty("Store", bindingFlags) ??
                throw new InvalidOperationException("Store property not found.");
            if (propertyInfo.GetValue(BlockChain) is IStore store)
            {
                return store;
            }

            throw new InvalidOperationException("Store property is not IStore.");
        }
    }

    public Swarm Swarm => _node!.Swarm;

    public IBlockChainIndex Index => throw new NotSupportedException();
}
