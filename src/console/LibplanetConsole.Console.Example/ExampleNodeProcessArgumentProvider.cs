namespace LibplanetConsole.Console.Example;

internal sealed class ExampleNodeProcessArgumentProvider
    : ProcessArgumentProviderBase<ExampleNode>
{
    protected override IEnumerable<string> GetArguments(ExampleNode obj)
    {
        if (obj.IsExample == true)
        {
            yield return "--example";
        }
    }
}
