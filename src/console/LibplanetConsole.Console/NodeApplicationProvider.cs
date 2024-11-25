using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class NodeApplicationProvider : InfoProviderBase<Node>
{
    public NodeApplicationProvider()
        : base("Application")
    {
    }

    protected override object? GetInfo(Node obj)
    {
        var options = obj.Options;
        return new
        {
            EndPoint = EndPointUtility.ToString(options.EndPoint),
            options.StorePath,
            options.LogPath,
            options.ActionProviderModulePath,
            options.ActionProviderType,
            options.BlocksyncPort,
            options.ConsensusPort,
            options.RepositoryPath,
        };
    }
}
