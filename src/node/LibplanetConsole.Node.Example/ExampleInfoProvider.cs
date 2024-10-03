using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Example;

internal sealed class ExampleInfoProvider(Example exampleNode)
    : InfoProviderBase<IApplication>(nameof(Example))
{
    protected override object? GetInfo(IApplication obj)
    {
        return new
        {
            exampleNode.IsExample,
        };
    }
}
