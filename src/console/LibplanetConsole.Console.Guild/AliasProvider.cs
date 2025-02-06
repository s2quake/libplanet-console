using System.Reflection;
using JSSoft.Commands;
using LibplanetConsole.Alias;
using Microsoft.Extensions.Hosting;
using Nekoyume;

namespace LibplanetConsole.Console.Guild;

internal sealed class AliasProvider(IAliasCollection aliases) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var fieldInfos = typeof(Addresses).GetFields(BindingFlags.Public | BindingFlags.Static)
           .Where(f => f.FieldType == typeof(Address));

        foreach (var fieldInfo in fieldInfos)
        {
            var address = (Address)fieldInfo.GetValue(null)!;
            var name = CommandUtility.ToSpinalCase(fieldInfo.Name);
            var aliasInfo = new AliasInfo
            {
                Alias = name,
                Address = address,
                Tags = ["lib9c"],
            };

            await aliases.AddAsync(aliasInfo, cancellationToken);
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
