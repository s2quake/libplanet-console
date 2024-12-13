namespace LibplanetConsole.Client.Bank;

internal sealed class CurrencyCollection(Bank bank) : CurrencyCollectionBase, IClientContent
{
    IEnumerable<IClientContent> IClientContent.Dependencies { get; } = [bank];

    string IClientContent.Name => "currencies";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var currencyInfos = await bank.GetCurrenciesAsync(cancellationToken);
        foreach (var currencyInfo in currencyInfos)
        {
            Add(currencyInfo.Code, currencyInfo.Currency);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Clear();
        await Task.CompletedTask;
    }
}
