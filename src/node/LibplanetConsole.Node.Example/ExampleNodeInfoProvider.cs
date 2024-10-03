using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Example;

internal sealed class ExampleNodeInfoProvider(ExampleNode exampleNode)
    : InfoProviderBase<IApplication>(nameof(ExampleNode))
{
    protected override object? GetInfo(IApplication obj)
    {
        return new
        {
            exampleNode.IsExample,
        };
    }
}
