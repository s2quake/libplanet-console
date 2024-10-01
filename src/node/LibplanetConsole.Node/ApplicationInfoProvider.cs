using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

[Export(typeof(IInfoProvider))]
internal sealed class ApplicationInfoProvider : InfoProviderBase<ApplicationBase>
{
    public ApplicationInfoProvider()
        : base("Application")
    {
    }

    protected override object? GetInfos(ApplicationBase obj) => obj.Info;
}
