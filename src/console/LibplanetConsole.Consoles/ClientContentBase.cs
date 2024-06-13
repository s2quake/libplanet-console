namespace LibplanetConsole.Consoles;

public abstract class ClientContentBase : IClientContent, IDisposable
{
    private readonly string _name;
    private bool _isDisposed;

    protected ClientContentBase(IClient client, string name)
    {
        _name = name;
        Client = client;
        Client.Attached += Client_Attached;
        Client.Detached += Client_Detached;
        Client.Started += Client_Started;
        Client.Stopped += Client_Stopped;
    }

    protected ClientContentBase(IClient client)
        : this(client, string.Empty)
    {
    }

    public IClient Client { get; }

    public string Name => _name != string.Empty ? _name : GetType().Name;

    void IDisposable.Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        Client.Attached -= Client_Attached;
        Client.Detached -= Client_Detached;
        Client.Started -= Client_Started;
        Client.Stopped -= Client_Stopped;
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    protected virtual void OnClientAttached()
    {
    }

    protected virtual void OnClientDetached()
    {
    }

    protected virtual void OnClientStarted()
    {
    }

    protected virtual void OnClientStopped()
    {
    }

    private void Client_Attached(object? sender, EventArgs e) => OnClientAttached();

    private void Client_Detached(object? sender, EventArgs e) => OnClientDetached();

    private void Client_Started(object? sender, EventArgs e) => OnClientStarted();

    private void Client_Stopped(object? sender, EventArgs e) => OnClientStopped();
}
