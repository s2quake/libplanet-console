using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node;

internal sealed class AddressCollection : AddressCollectionBase
{
    public AddressCollection()
    {
    }

    public void Collect(IServiceProvider serviceProvider)
    {
        var addressProviders = serviceProvider.GetServices<IAddressProvider>();
        var items = addressProviders.SelectMany(provider => provider.Addresses);
        foreach (var item in items)
        {
            Add(item.Alias, item.Address);
        }
    }
}
