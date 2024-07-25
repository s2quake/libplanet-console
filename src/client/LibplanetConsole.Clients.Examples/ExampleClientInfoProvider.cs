using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Examples;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ExampleClientInfoProvider(ExampleClient exampleClient)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (
            Name: nameof(ExampleClient),
            Value: new
            {
                exampleClient.IsExample,
            });
    }
}
