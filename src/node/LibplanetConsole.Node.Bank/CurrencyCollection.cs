namespace LibplanetConsole.Node.Bank;

internal sealed class CurrencyCollection(IEnumerable<ICurrencyProvider> currencyProviders)
    : CurrencyCollectionBase, INodeContent
{
    private readonly ICurrencyProvider[] _currencyProviders = [.. currencyProviders];

    string INodeContent.Name => "currencyes";

    IEnumerable<INodeContent> INodeContent.Dependencies { get; } = [];

    async Task INodeContent.StartAsync(CancellationToken cancellationToken)
    {
        var items = _currencyProviders.SelectMany(provider => provider.Currencies);
        foreach (var item in items)
        {
            Add(item.Code, item.Currency);
        }

        await Task.CompletedTask;
    }

    async Task INodeContent.StopAsync(CancellationToken cancellationToken)
    {
        Clear();

        await Task.CompletedTask;
    }
}
