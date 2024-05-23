using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ApplicationInfoProvider(ApplicationBase application) : IInfoProvider
{
    public Type DeclaringType => typeof(IApplication);

    public IEnumerable<(string Name, object? Value)> GetInfos()
    {
        var info = application.Info;
        yield return (nameof(info.EndPoint), info.EndPoint);
        yield return (nameof(info.NodeEndPoint), info.NodeEndPoint);
    }
}
