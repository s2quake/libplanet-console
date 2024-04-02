using Libplanet.Net;

namespace LibplanetConsole.Executable.Serializations;

record class NodeInfo
{
    public NodeInfo(Swarm swarm)
    {
        AppProtocolVersion = $"{swarm.AppProtocolVersion}";
        EndPoint = $"{swarm.EndPoint.Host}:{swarm.EndPoint.Port}";
        Address = $"{swarm.Address}";
        ConsensusRunning = $"{swarm.ConsensusRunning}";
        Running = $"{swarm.Running}";
        LastMessageTimestamp = $"{swarm.LastMessageTimestamp}";
        BlockChain = new(swarm.BlockChain);
        Peers = swarm.Peers.Select(item => new BoundPeerInfo(item)).ToArray();
        Validators = swarm.Validators != null ? swarm.Validators.Select(item => new BoundPeerInfo(item)).ToArray() : [];
        TrustedAppProtocolVersionSigners = swarm.TrustedAppProtocolVersionSigners.Select(item => $"{item}").ToArray();
    }

    public string AppProtocolVersion { get; }

    public string EndPoint { get; }

    public string Address { get; }

    public string ConsensusRunning { get; }

    public string Running { get; }

    public string LastMessageTimestamp { get; }

    public string[] TrustedAppProtocolVersionSigners { get; }

    public BlockChainInfo BlockChain { get; }

    public BoundPeerInfo[] Peers { get; }

    public BoundPeerInfo[] Validators { get; }
}
