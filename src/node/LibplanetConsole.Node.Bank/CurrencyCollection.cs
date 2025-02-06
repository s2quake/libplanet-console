using LibplanetConsole.Bank;

namespace LibplanetConsole.Node.Bank;

internal sealed class CurrencyCollection(INode node)
    : CurrencyCollectionBase, INodeContent
{
    string INodeContent.Name => "currencies";

    IEnumerable<INodeContent> INodeContent.Dependencies { get; } = [];

    int INodeContent.Order => int.MaxValue;

    async Task INodeContent.StartAsync(CancellationToken cancellationToken)
    {
        var currencyProviders = node.GetServices<ICurrencyProvider>();
        var items = currencyProviders.SelectMany(provider => provider.Currencies);
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
