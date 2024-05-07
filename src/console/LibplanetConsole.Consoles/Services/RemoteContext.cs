using JSSoft.Communication;
using LibplanetConsole.Consoles.Services;

namespace LibplanetConsole.Consoles.Services;

public sealed class RemoteContext(object owner, object[] items)
    : ClientContext(GetServices(owner, items))
{
    private static IService[] GetServices(object obj, Array items)
    {
        var serviceList = new List<IService>(items.Length + 1);
        {
            if (obj is IRemoteServiceProvider remoteServiceProvider)
            {
                serviceList.Add(remoteServiceProvider.GetService(obj));
            }
        }

        foreach (var item in items)
        {
            if (item is IRemoteServiceProvider remoteServiceProvider)
            {
                serviceList.Add(remoteServiceProvider.GetService(obj));
            }
        }

        return [.. serviceList];
    }
}
