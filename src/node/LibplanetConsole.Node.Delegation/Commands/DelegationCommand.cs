using System.ComponentModel;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Provides commands for the delegation service.")]
[Category("Delegation")]
internal sealed class DelegationCommand(INode node, IDelegation delegation)
    : CommandMethodBase()
{
}
