namespace LibplanetConsole.Client;

public sealed class ClientEventArgs(ClientInfo clientInfo) : EventArgs
{
    public ClientInfo ClientInfo { get; } = clientInfo;
}
