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
            Url = options.Url.ToString(),
            NodeUrl = options.HubUrl?.ToString(),
            options.LogPath,
            options.RepositoryPath,
        };
    }
}
