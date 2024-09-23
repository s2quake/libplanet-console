using LibplanetConsole.Common;

namespace LibplanetConsole.Example.Services;

public interface IExampleNodeService
{
    void Subscribe(AppAddress address);

    void Unsubscribe(AppAddress address);

    Task<int> GetAddressCountAsync(CancellationToken cancellationToken);

    Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken);
}
