using LibplanetConsole.BlockChain;

namespace LibplanetConsole.Node;

internal sealed class AddressCollection(IEnumerable<IAddressProvider> addressProviders)
    : AddressCollectionBase, INodeContent
{
    private readonly IAddressProvider[] _addressProviders = [.. addressProviders];

    string INodeContent.Name => "addresses";

    IEnumerable<INodeContent> INodeContent.Dependencies { get; } = [];

    async Task INodeContent.StartAsync(CancellationToken cancellationToken)
    {
        var addressInfos = _addressProviders.SelectMany(provider => provider.AddressInfos);
        foreach (var addressInfo in addressInfos)
        {
            Add(addressInfo);
        }

        await Task.CompletedTask;
    }

    async Task INodeContent.StopAsync(CancellationToken cancellationToken)
    {
        Clear();

        await Task.CompletedTask;
    }
}
