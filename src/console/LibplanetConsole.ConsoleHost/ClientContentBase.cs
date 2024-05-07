using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost;

public abstract class ClientContentBase(IClient client) : IClientContent
{
    public IClient Client { get; } = client;
}
