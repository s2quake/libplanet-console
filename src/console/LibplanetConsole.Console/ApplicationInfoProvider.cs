using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class ApplicationInfoProvider : InfoProviderBase<ApplicationBase>
{
    public ApplicationInfoProvider()
        : base("Application")
    {
    }

    protected override object? GetInfo(ApplicationBase obj) => obj.Info;
}
