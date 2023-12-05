using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Blockchain;
using Newtonsoft.Json;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class BlockCommand(Application application) : CommandMethodBase
{
    private readonly Application _application = application;
    private readonly BlockChain _blockChain = application.GetService<BlockChain>()!;
    private readonly UserCollection _users = application.GetService<UserCollection>()!;

    [CommandProperty]
    public int UserIndex { get; set; }

    [CommandMethod]
    public void New()
    {
        var userIndex = UserIndex;
        var user = _users[userIndex];
        var block = BlockChainUtils.AppendNew(_blockChain, user, _users, Array.Empty<IValue>());
        var sb = new StringBuilder();
        sb.AppendStatesLine(_blockChain, block.Index, _users);
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public void List()
    {
        var blockChainInfo = new BlockChainInfo(_blockChain);
        var json = JsonConvert.SerializeObject(blockChainInfo, Formatting.Indented);
        Out.Write(json);
    }
}
