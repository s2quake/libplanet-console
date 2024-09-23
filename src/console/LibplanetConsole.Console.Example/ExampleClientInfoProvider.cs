using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

[Export(typeof(IInfoProvider))]
internal sealed class ExampleClientInfoProvider
    : InfoProviderBase<ExampleClientContent>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(ExampleClientContent obj)
    {
        yield return (nameof(obj.IsExample), obj.IsExample);
    }
}
