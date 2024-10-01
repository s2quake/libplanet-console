using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

[Export(typeof(IInfoProvider))]
internal sealed class ExampleClientInfoProvider
    : InfoProviderBase<ExampleClientContent>
{
    public ExampleClientInfoProvider()
        : base(nameof(ExampleClientContent))
    {
    }

    protected override object? GetInfos(ExampleClientContent obj)
    {
        return new
        {
            obj.IsExample,
        };
    }
}
