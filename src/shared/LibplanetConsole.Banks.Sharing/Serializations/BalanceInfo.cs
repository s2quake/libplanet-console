using LibplanetConsole.Common;

namespace LibplanetConsole.Banks.Serializations;

public readonly partial record struct BalanceInfo
{
    public BalanceInfo()
    {
    }

    public AppAddress Address { get; init; }

    public string Governance { get; init; } = string.Empty;

    public string Consensus { get; init; } = string.Empty;

    public string Share { get; init; } = string.Empty;
}
