namespace LibplanetConsole.Client.Services;

public interface IClientCallback
{
    void OnStarted(ClientInfo clientInfo);

    void OnStopped();
}
