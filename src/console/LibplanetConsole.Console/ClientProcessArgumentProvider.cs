using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

internal sealed class ClientProcessArgumentProvider : ProcessArgumentProviderBase<Client>
{
    protected override IEnumerable<string> GetArguments(Client obj)
    {
        var contents = obj.GetRequiredService<IEnumerable<IClientContent>>();
        foreach (var content in contents)
        {
            var args = ProcessEnvironment.GetArguments(serviceProvider: obj, obj: content);
            foreach (var arg in args)
            {
                yield return arg;
            }
        }
    }
}
