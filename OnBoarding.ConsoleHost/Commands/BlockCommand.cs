using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using Libplanet.Blockchain;
using OnBoarding.ConsoleHost.Extensions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class BlockCommand(Application application) : CommandMethodBase
{
    private readonly Application _application = application;
    private readonly BlockChain _blockChain = application.GetService<BlockChain>()!;
    private readonly UserCollection _users = application.GetService<UserCollection>()!;
    private readonly ActionCollection _actions = application.GetService<ActionCollection>()!;

    [CommandProperty]
    public int UserIndex { get; set; }

    [CommandMethod]
    public void New()
    {
        var userIndex = UserIndex;
        var user = _users[userIndex];
        var block = BlockChainUtils.AppendNew(_blockChain, user, _users, [.. _actions]);
        var sb = new StringBuilder();
        sb.AppendStatesLine(_blockChain, block.Index, _users);
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        sb.AppendStatesLine(_blockChain, _users);
        Out.Write(sb.ToString());
    }
}
