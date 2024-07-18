using System.ComponentModel.Composition;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using Nekoyume.Action.Guild;

namespace LibplanetConsole.Nodes.Executable;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class GuildEventTracer(INode node) : IApplicationService
{
    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        node.BlockAppended += Node_BlockAppended;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        node.BlockAppended -= Node_BlockAppended;
        return ValueTask.CompletedTask;
    }

    private static bool ContainsAction(BlockInfo blockInfo, string typeId)
    {
        var actions = blockInfo.Transactions.SelectMany(item => item.Actions);
        return actions.Any(item => item.TypeId == typeId);
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        if (ContainsAction(blockInfo, MakeGuild.TypeIdentifier) == true)
        {
            Console.Out.WriteLine("Guild created");
        }

        if (ContainsAction(blockInfo, RemoveGuild.TypeIdentifier) == true)
        {
            Console.Out.WriteLine("Guild deleted.");
        }
    }
}
