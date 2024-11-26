using Libplanet.Action.State;
using Libplanet.Types.Evidence;

namespace LibplanetConsole.Node.Delegation;

internal sealed class DummayActionContext : IActionContext
{
    private IWorld? _previousState;

    public Address Signer { get; set; }

    public TxId? TxId { get; set; }

    public Address Miner { get; set; }

    public long BlockIndex { get; set; }

    public int BlockProtocolVersion { get; set; }

    public BlockCommit? LastCommit { get; set; }

    public IWorld PreviousState
    {
        get => _previousState ?? throw new InvalidOperationException("There is no previous state.");
        set => _previousState = value;
    }

    public int RandomSeed { get; set; }

    public bool IsPolicyAction { get; set; }

    public FungibleAssetValue? MaxGasPrice { get; set; }

    public IReadOnlyList<ITransaction> Txs { get; set; } = [];

    public IReadOnlyList<EvidenceBase> Evidence { get; set; } = [];

    public IRandom GetRandom()
    {
        throw new NotSupportedException("This method is not supported.");
    }
}
