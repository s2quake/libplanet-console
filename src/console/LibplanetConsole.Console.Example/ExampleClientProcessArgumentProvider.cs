using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

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
