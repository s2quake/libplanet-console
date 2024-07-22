using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Evidence.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides evidence-related commands.")]
[Category("Evidence")]
[method: ImportingConstructor]
internal sealed class EvidenceCommand(INode node, IEvidenceNode evidenceNode)
    : CommandMethodBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandMethod]
    [CommandSummary("Adds a new evidence.")]
    public async Task NewAsync(CancellationToken cancellationToken)
    {
        var evidenceInfo = await evidenceNode.AddEvidenceAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo);
    }

    [CommandMethod]
    [CommandSummary("Raises a infraction.")]
    public async Task RaiseAsync(CancellationToken cancellationToken)
    {
        await evidenceNode.ViolateAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(ListProperties))]
    [CommandSummary("Gets the evidence list.")]
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var height = ListProperties.Height;
        var isPending = ListProperties.IsPending;
        var evidenceInfos = isPending == true ?
            await evidenceNode.GetPendingEvidenceAsync(cancellationToken) :
            await evidenceNode.GetEvidenceAsync(height, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfos);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(GetProperties))]
    [CommandSummary("Gets the evidence.")]
    public async Task GetAsync(string evidenceId, CancellationToken cancellationToken)
    {
        var isPending = GetProperties.IsPending;
        var evidenceInfo = isPending == true ?
            await evidenceNode.GetPendingEvidenceAsync(evidenceId, cancellationToken) :
            await evidenceNode.GetEvidenceAsync(evidenceId, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo);
    }

#if LIBPLANET_DPOS
    [CommandMethod]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        await evidenceNode.UnjailAsync(cancellationToken);
    }
#endif // LIBPLANET_DPOS

    public static class ListProperties
    {
        [CommandPropertyRequired(DefaultValue = -1)]
        [CommandSummary("The height of the block to get the evidence. default is the tip.")]
        public static long Height { get; set; }

        [CommandPropertySwitch("pending", 'p')]
        [CommandPropertyCondition(nameof(Height), -1, IsNot = true)]
        [CommandSummary("Indicates whether to get pending evidence. " +
                        "if true, the height is ignored.")]
        public static bool IsPending { get; set; }
    }

    public static class GetProperties
    {
        [CommandPropertySwitch("pending", 'p')]
        [CommandSummary("Indicates whether to get pending evidence.")]
        public static bool IsPending { get; set; }
    }
}
