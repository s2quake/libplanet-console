using JSSoft.Communication;

namespace LibplanetConsole.Executable;

public interface IRemoteServiceProvider
{
    IService GetService(object obj);
}
