namespace LibplanetConsole.Clients.Services;

public interface IClientCallback
{
    void OnStarted(ClientInfo clientInfo);

    void OnStopped();
}
