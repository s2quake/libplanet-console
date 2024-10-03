using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

internal sealed class ExampleClientInfoProvider
    : InfoProviderBase<ExampleClientContent>
{
    public ExampleClientInfoProvider()
        : base(nameof(ExampleClientContent))
    {
    }

    protected override object? GetInfo(ExampleClientContent obj)
    {
        return new
        {
            obj.IsExample,
        };
    }
}
