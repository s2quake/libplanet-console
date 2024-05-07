namespace LibplanetConsole.Clients.Serializations;

public record class ClientInfo
{
    public ClientInfo()
    {
    }

    public string PrivateKey { get; init; } = string.Empty;

    public string PublicKey { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;
}
