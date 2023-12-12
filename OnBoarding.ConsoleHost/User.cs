using Bencodex.Types;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost;

sealed class User
{
    private readonly PrivateKey _privateKey = new();

    public User(string name)
    {
        Address = _privateKey.ToAddress();
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => $"{_privateKey.PublicKey}";

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address { get; }

    public PlayerInfo GetPlayerInfo(SwarmHost swarmHost) => GetPlayerInfo(swarmHost, blockIndex: -1);

    public PlayerInfo GetPlayerInfo(SwarmHost swarmHost, int blockIndex)
    {
        var blockChain = swarmHost.BlockChain;
        var address = Address;
        var actualBlockIndex = blockIndex == -1 ? blockChain.Count - 1 : blockIndex;
        var block = blockChain[actualBlockIndex];
        var worldState = blockChain.GetWorldState(block.Hash);
        var account = worldState.GetAccount(address);
        if (account.GetState(address) is Dictionary values)
        {
            return new PlayerInfo(values);
        }
        return PlayerInfo.CreateNew(Name, address);
    }
}
