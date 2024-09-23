using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

[Export(typeof(IInfoProvider))]
internal sealed class ApplicationInfoProvider : InfoProviderBase<ApplicationBase>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(ApplicationBase obj)
        => InfoUtility.EnumerateValues(obj.Info);
}
