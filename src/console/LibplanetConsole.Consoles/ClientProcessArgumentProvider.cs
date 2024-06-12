using System.ComponentModel.Composition;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles;

[Export(typeof(IProcessArgumentProvider))]
internal sealed class ClientProcessArgumentProvider : ProcessArgumentProviderBase<Client>
{
    protected override IEnumerable<string> GetArguments(Client obj)
    {
        var contents = obj.GetService<IEnumerable<IClientContent>>();
        foreach (var content in contents)
        {
            var contentArguments = ProcessUtility.GetArguments(serviceProvider: obj, obj: content);
            foreach (var item in contentArguments)
            {
                yield return item;
            }
        }
    }
}
