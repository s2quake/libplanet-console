#if LIBPLANET_NODE
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes;

public sealed partial record class NodeOptions
{
    public static NodeOptions Create(AppPublicKey publicKey)
    {
        var genesisKey = new AppPrivateKey();
        var genesisValidators = new AppPublicKey[] { publicKey };
        var timestamp = DateTimeOffset.UtcNow;
        var genesisBlock = BlockUtility.CreateGenesisBlock(
            genesisKey, genesisValidators, timestamp);
        return new NodeOptions
        {
            Genesis = BlockUtility.SerializeBlock(genesisBlock),
        };
    }

    public static NodeOptions Create(AppPublicKey[] validatorKeys)
    {
        var genesisKey = GenesisOptions.AppProtocolKey;
        var timestamp = DateTimeOffset.UtcNow;
        var genesisBlock = BlockUtility.CreateGenesisBlock(
            genesisKey, validatorKeys, timestamp);
        return new NodeOptions
        {
            Genesis = BlockUtility.SerializeBlock(genesisBlock),
        };
    }
}

#endif // LIBPLANET_NODE
