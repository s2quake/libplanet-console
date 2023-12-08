using System.Net;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Net;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost;

sealed class User
{
    private readonly PrivateKey _privateKey = new();

    public User(string name, int peerPort, int consensusPeerPort)
    {
        Address = _privateKey.ToAddress();
        Name = name;
        Peer = new BoundPeer(_privateKey.PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", peerPort));
        ConsensusPeer = new BoundPeer(_privateKey.PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", consensusPeerPort));
    }

    public string Name { get; }

    public override string ToString() => $"{_privateKey.PublicKey}";

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address { get; }

    public BoundPeer Peer { get; }

    public BoundPeer ConsensusPeer { get; }

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
