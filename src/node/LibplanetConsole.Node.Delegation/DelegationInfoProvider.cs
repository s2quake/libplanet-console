using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Delegation;

internal sealed class DelegationInfoProvider(Delegation delegation)
    : InfoProviderBase<IHostApplicationLifetime>(nameof(Delegation))
{
    protected override object? GetInfo(IHostApplicationLifetime obj) => delegation.Info;
}
