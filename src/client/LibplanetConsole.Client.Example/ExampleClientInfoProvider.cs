using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Example;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ExampleClientInfoProvider(ExampleClient exampleClient)
    : InfoProviderBase<IApplication>(nameof(ExampleClient))
{
    protected override object? GetInfos(IApplication obj)
    {
        return new
        {
            exampleClient.IsExample,
        };
    }
}
