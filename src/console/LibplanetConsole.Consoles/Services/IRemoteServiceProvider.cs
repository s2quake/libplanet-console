using JSSoft.Communication;

namespace LibplanetConsole.Consoles.Services;

public interface IRemoteServiceProvider
{
    IService GetService(object obj);
}
