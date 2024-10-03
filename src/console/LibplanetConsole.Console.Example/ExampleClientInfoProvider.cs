using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Example;

internal sealed class ExampleClientInfoProvider
    : InfoProviderBase<ExampleClient>
{
    public ExampleClientInfoProvider()
        : base(nameof(ExampleClient))
    {
    }

    protected override object? GetInfo(ExampleClient obj)
    {
        return new
        {
            obj.IsExample,
        };
    }
}
