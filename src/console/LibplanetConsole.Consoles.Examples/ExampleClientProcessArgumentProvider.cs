using System.ComponentModel.Composition;

namespace LibplanetConsole.Consoles.Examples;

[Export(typeof(IProcessArgumentProvider))]
internal sealed class ExampleClientProcessArgumentProvider
    : ProcessArgumentProviderBase<ExampleClientContent>
{
    protected override IEnumerable<string> GetArguments(ExampleClientContent obj)
    {
        if (obj.IsExample == true)
        {
            yield return "--example";
        }
    }
}
