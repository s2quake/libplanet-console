namespace LibplanetConsole.Node;

internal sealed class AddressCollection(IEnumerable<IAddressProvider> addressProviders)
    : AddressCollectionBase, INodeContent
{
    private readonly IAddressProvider[] _addressProviders = [.. addressProviders];

    string INodeContent.Name => "addresses";

    IEnumerable<INodeContent> INodeContent.Dependencies { get; } = [];

    async Task INodeContent.StartAsync(CancellationToken cancellationToken)
    {
        var items = _addressProviders.SelectMany(provider => provider.Addresses);
        foreach (var item in items)
        {
            Add(item.Alias, item.Address);
        }

        await Task.CompletedTask;
    }

    async Task INodeContent.StopAsync(CancellationToken cancellationToken)
    {
        Clear();

        await Task.CompletedTask;
    }
}
