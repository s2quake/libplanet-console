using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Consoles.Services;

public interface INodeContentService
{
    IRemoteService RemoteService { get; }
}
