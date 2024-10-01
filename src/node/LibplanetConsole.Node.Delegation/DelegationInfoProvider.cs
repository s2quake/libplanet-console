using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Delegation;

internal sealed class DelegationInfoProvider(Delegation delegation)
    : InfoProviderBase<IApplication>(nameof(Delegation))
{
    protected override object? GetInfo(IApplication obj) => delegation.Info;
}
