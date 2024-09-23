using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Examples;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ExampleNodeInfoProvider(ExampleNode exampleNode)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (nameof(ExampleNode), new
        {
            exampleNode.IsExample,
        });
    }
}
