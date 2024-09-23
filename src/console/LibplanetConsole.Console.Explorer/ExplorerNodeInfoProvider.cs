using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles.Explorer;

[Export(typeof(IInfoProvider))]
internal sealed class ExplorerNodeInfoProvider
    : InfoProviderBase<ExplorerNodeContent>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(ExplorerNodeContent obj)
    {
        yield return (nameof(obj.EndPoint), obj.EndPoint);
        yield return (nameof(obj.IsRunning), obj.IsRunning);
        yield return ("Url", $"http://{obj.EndPoint}/ui/playground");
    }
}
