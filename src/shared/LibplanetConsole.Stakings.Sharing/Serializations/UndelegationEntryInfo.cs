namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct UndelegationEntryInfo
{
    public UndelegationEntryInfo()
    {
    }

    public string InitialConsensusToken { get; init; } = string.Empty;

    public string UnbondingConsensusToken { get; init; } = string.Empty;

    public long CreationHeight { get; init; }
}
