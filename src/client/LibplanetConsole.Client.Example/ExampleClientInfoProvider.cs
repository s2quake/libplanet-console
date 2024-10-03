using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Example;

internal sealed class ExampleClientInfoProvider(ExampleClient exampleClient)
    : InfoProviderBase<IApplication>(nameof(ExampleClient))
{
    protected override object? GetInfo(IApplication obj)
    {
        return new
        {
            exampleClient.IsExample,
        };
    }
}
