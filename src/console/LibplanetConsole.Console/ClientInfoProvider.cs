using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

[Export(typeof(IInfoProvider))]
internal sealed class ClientInfoProvider : InfoProviderBase<Client>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(Client obj)
    {
        foreach (var item in InfoUtility.EnumerateValues(obj.Info))
        {
            yield return item;
        }

        var contents = obj.GetRequiredService<IEnumerable<IClientContent>>();

        foreach (var content in contents)
        {
            var contentInfos = InfoUtility.GetInfo(serviceProvider: obj, obj: content);
            if (contentInfos.Count > 0)
            {
                yield return (content.Name, contentInfos);
            }
        }
    }
}
