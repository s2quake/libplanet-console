using System.Collections.Immutable;
using System.Security.Cryptography;
using Libplanet.Action.State;
using Libplanet.Types.Evidence;

namespace LibplanetConsole.Node.Guild;

internal sealed class ActionContext : IActionContext
{
    private IWorld? _previousState;

    private IRandom? _random = null;

    private IReadOnlyList<ITransaction>? _txs = null;

    private IReadOnlyList<EvidenceBase>? _evs = null;

    public BlockHash? GenesisHash { get; set; }

    public Address Signer { get; set; }

    public TxId? TxId { get; set; }

    public Address Miner { get; set; }

    public BlockHash BlockHash { get; set; }

    public long BlockIndex { get; set; }

    public int BlockProtocolVersion { get; set; } = BlockMetadata.CurrentProtocolVersion;

    public BlockCommit? LastCommit { get; set; }

    public IWorld PreviousState
    {
        get => _previousState ?? throw new InvalidOperationException("PreviousState is not set.");
        set => _previousState = value;
    }

    public int RandomSeed { get; set; }

    public HashDigest<SHA256>? PreviousStateRootHash { get; set; }

    public bool IsPolicyAction { get; set; }

    public FungibleAssetValue? MaxGasPrice { get; set; }

    public IReadOnlyList<ITransaction> Txs
    {
        get => _txs ?? ImmutableList<ITransaction>.Empty;
        set => _txs = value;
    }

    public IReadOnlyList<EvidenceBase> Evidence
    {
        get => _evs ?? ImmutableList<EvidenceBase>.Empty;
        set => _evs = value;
    }

    public IRandom GetRandom() => _random ?? new Random(0);

    public void SetRandom(IRandom random)
    {
        _random = random;
    }

    private sealed class Random(int seed) : System.Random(seed), IRandom
    {
        public int Seed { get; } = seed;
    }
}
