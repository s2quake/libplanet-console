using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles;

[Export(typeof(IInfoProvider))]
internal sealed class NodeInfoProvider : InfoProviderBase<Node>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(Node obj)
    {
        foreach (var item in InfoUtility.EnumerateValues(obj.Info))
        {
            yield return item;
        }

        var contents = obj.GetService<IEnumerable<INodeContent>>();

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
