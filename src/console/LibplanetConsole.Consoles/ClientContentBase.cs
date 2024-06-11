namespace LibplanetConsole.Consoles;

public abstract class ClientContentBase(IClient client, string name) : IClientContent
{
    public ClientContentBase(IClient client)
        : this(client, client.GetType().Name)
    {
    }

    public IClient Client { get; } = client;

    public string Name { get; } = name;
}
