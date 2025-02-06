using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Alias;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Hand.Commands;

[CommandSummary("Provides hand-related commands")]
[Category("Hand")]
internal sealed class HandCommand(
    INode node,
    IHand hand,
    IAliasCollection aliases)
    : CommandMethodBase
{

}
