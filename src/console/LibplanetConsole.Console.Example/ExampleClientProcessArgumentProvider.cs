namespace LibplanetConsole.Console.Example;

internal sealed class ExampleClientProcessArgumentProvider
    : ProcessArgumentProviderBase<ExampleClient>
{
    protected override IEnumerable<string> GetArguments(ExampleClient obj)
    {
        if (obj.IsExample == true)
        {
            yield return "--example";
        }
    }
}
