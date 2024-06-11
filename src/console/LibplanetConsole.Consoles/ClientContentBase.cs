namespace LibplanetConsole.Consoles;

public abstract class ClientContentBase(IClient client, string name) : IClientContent
{
    public ClientContentBase(IClient client)
        : this(client, string.Empty)
    {
    }

    public IClient Client { get; } = client;

    public string Name => name != string.Empty ? name : GetType().Name;
}
