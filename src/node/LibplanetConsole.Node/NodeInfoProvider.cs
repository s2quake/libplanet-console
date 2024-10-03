using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

[Export(typeof(IInfoProvider))]
internal sealed class NodeInfoProvider(Node node) : InfoProviderBase<ApplicationBase>(nameof(Node))
{
    protected override object? GetInfo(ApplicationBase obj) => node.Info;
}
