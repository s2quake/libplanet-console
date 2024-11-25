using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class ClientApplicationProvider : InfoProviderBase<Client>
{
    public ClientApplicationProvider()
        : base("Application")
    {
    }

    protected override object? GetInfo(Client obj)
    {
        var options = obj.Options;
        return new
        {
            EndPoint = EndPointUtility.ToString(options.EndPoint),
            NodeEndPoint = EndPointUtility.ToString(options.NodeEndPoint),
            options.LogPath,
            options.RepositoryPath,
        };
    }
}
