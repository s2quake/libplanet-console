using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Explorer;

internal sealed class ExplorerInfoProvider : InfoProviderBase<ExplorerNode>
{
    public ExplorerInfoProvider()
        : base("Explorer")
    {
    }

    protected override object? GetInfo(ExplorerNode obj)
    {
        return new
        {
            obj.EndPoint,
            obj.IsRunning,
            Url = obj.IsRunning ? $"http://{obj.EndPoint}/ui/playground" : string.Empty,
        };
    }
}
