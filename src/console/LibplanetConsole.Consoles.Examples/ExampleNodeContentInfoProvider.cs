using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles.Examples;

[Export(typeof(IInfoProvider))]
internal sealed class ExampleNodeContentInfoProvider
    : InfoProviderBase<ExampleNodeContent>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(ExampleNodeContent obj)
    {
        yield return (nameof(obj.IsExample), obj.IsExample);
    }
}
