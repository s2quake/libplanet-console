using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Consoles.Services;

public interface IClientContentService
{
    IRemoteService RemoteService { get; }
}
