using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

[Export(typeof(IInfoProvider))]
internal sealed class ExampleNodeInfoProvider
    : InfoProviderBase<ExampleNodeContent>
{
    public ExampleNodeInfoProvider()
        : base(nameof(ExampleNodeContent))
    {
    }

    protected override object? GetInfo(ExampleNodeContent obj)
    {
        return new
        {
            obj.IsExample,
        };
    }
}
