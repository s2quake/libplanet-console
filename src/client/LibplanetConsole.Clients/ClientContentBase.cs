namespace LibplanetConsole.Clients;

public abstract class ClientContentBase(IClient client) : IClientContent
{
    public IClient Client { get; } = client;
}
