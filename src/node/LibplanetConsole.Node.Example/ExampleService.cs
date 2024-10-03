using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Example.Services;

namespace LibplanetConsole.Node.Example;

internal sealed class ExampleService : LocalService<IExampleNodeService, IExampleNodeCallback>,
    IExampleNodeService, IDisposable
{
    private readonly Example _example;

    public ExampleService(Example example)
    {
        _example = example;
        _example.Subscribed += Example_Subscribed;
        _example.Unsubscribed += Example_Unsubscribed;
    }

    public void Dispose()
    {
        _example.Subscribed -= Example_Subscribed;
        _example.Unsubscribed -= Example_Unsubscribed;
    }

    public async Task<int> GetAddressCountAsync(CancellationToken cancellationToken)
    {
        var addresses = await _example.GetAddressesAsync(cancellationToken);
        return addresses.Length;
    }

    public Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken)
        => _example.GetAddressesAsync(cancellationToken);

    public void Subscribe(Address address) => _example.Subscribe(address);

    public void Unsubscribe(Address address) => _example.Unsubscribe(address);

    private void Example_Subscribed(object? sender, ItemEventArgs<Address> e)
        => Callback.OnSubscribed(e.Item);

    private void Example_Unsubscribed(object? sender, ItemEventArgs<Address> e)
        => Callback.OnUnsubscribed(e.Item);
}
