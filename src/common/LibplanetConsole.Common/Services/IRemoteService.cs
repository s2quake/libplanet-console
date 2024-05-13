using JSSoft.Communication;

namespace LibplanetConsole.Common.Services;

public interface IRemoteService
{
    IService Service { get; }
}