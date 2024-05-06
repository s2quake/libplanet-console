using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Serializations;

public record class ClientInfo
{
    public ClientInfo(ClientBase clientBase)
    {
        PrivateKey = PrivateKeyUtility.ToString(clientBase.PrivateKey);
        PublicKey = PublicKeyUtility.ToString(clientBase.PublicKey);
        Address = $"{clientBase.Address}";
    }

    public ClientInfo()
    {
    }

    public string PrivateKey { get; init; } = string.Empty;

    public string PublicKey { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;
}
