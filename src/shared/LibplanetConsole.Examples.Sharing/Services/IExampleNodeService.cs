using LibplanetConsole.Common;

namespace LibplanetConsole.Examples.Services;

public interface IExampleNodeService
{
    void Subscribe(AppAddress address);

    void Unsubscribe(AppAddress address);

    Task<int> GetAddressCountAsync(CancellationToken cancellationToken);

    Task<AppAddress[]> GetAddressesAsync(CancellationToken cancellationToken);
}
