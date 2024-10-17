using System.ComponentModel;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(IBank bank) : CommandMethodBase
{
    
}
