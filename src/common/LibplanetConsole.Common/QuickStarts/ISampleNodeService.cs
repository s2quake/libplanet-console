using JSSoft.Communication;

namespace LibplanetConsole.Common.QuickStarts;

public interface ISampleNodeService
{
    [ServerMethod(IsOneWay = true)]
    void Subscribe(string address);

    [ServerMethod(IsOneWay = true)]
    void Unsubscribe(string address);

    [ServerMethod]
    int GetAddressCount();

    [ServerMethod]
    Task<string[]> GetAddressesAsync(CancellationToken cancellationToken);
}
