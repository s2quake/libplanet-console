using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Example;

internal sealed class ExampleInfoProvider(Example exampleClient)
    : InfoProviderBase<IApplication>(nameof(Example))
{
    protected override object? GetInfo(IApplication obj)
    {
        return new
        {
            exampleClient.IsExample,
        };
    }
}
