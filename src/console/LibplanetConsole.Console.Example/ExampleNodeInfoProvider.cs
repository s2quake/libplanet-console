using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

internal sealed class ExampleNodeInfoProvider
    : InfoProviderBase<ExampleNode>
{
    public ExampleNodeInfoProvider()
        : base(nameof(ExampleNode))
    {
    }

    protected override object? GetInfo(ExampleNode obj)
    {
        return new
        {
            obj.IsExample,
        };
    }
}
