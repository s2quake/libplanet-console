using System.ComponentModel.Composition;

namespace LibplanetConsole.Consoles.Examples;

[Export(typeof(IProcessArgumentProvider))]
internal sealed class ExampleNodeProcessArgumentProvider
    : ProcessArgumentProviderBase<ExampleNodeContent>
{
    protected override IEnumerable<string> GetArguments(ExampleNodeContent obj)
    {
        if (obj.IsExample == true)
        {
            yield return "--example";
        }
    }
}
