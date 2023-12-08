using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;

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
}
