namespace LibplanetConsole.Bank;

public readonly partial record struct BalanceInfo
{
    public BalanceInfo()
    {
    }

    public Address Address { get; init; }

    public string Gold { get; init; } = string.Empty;
}
