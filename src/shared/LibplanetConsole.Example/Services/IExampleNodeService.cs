namespace LibplanetConsole.Example.Services;

public interface IExampleNodeService
{
    void Subscribe(Address address);

    void Unsubscribe(Address address);

    Task<int> GetAddressCountAsync(CancellationToken cancellationToken);

    Task<Address[]> GetAddressesAsync(CancellationToken cancellationToken);
}
