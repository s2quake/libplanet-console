using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Evidence.Commands;

[CommandSummary("Provides evidence-related commands.")]
[Category("Evidence")]
internal sealed class EvidenceCommand(INode node, IEvidence evidence)
    : CommandMethodBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandMethod]
    [CommandSummary("Adds a new evidence.")]
    public async Task NewAsync(CancellationToken cancellationToken)
    {
        var evidenceInfo = await evidence.AddEvidenceAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo);
    }

    [CommandMethod]
    [CommandSummary("Raises a infraction.")]
    public async Task RaiseAsync(CancellationToken cancellationToken)
    {
        await evidence.ViolateAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(ListProperties))]
    [CommandSummary("Gets the evidence list.")]
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var height = ListProperties.Height;
        var isPending = ListProperties.IsPending;
        var evidenceInfos = isPending == true ?
            await evidence.GetPendingEvidenceAsync(cancellationToken) :
            await evidence.GetEvidenceAsync(height, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfos);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(GetProperties))]
    [CommandSummary("Gets the evidence.")]
    public async Task GetAsync(string evidenceId, CancellationToken cancellationToken)
    {
        var isPending = GetProperties.IsPending;
        var evidenceInfo = isPending == true ?
            await evidence.GetPendingEvidenceAsync(evidenceId, cancellationToken) :
            await evidence.GetEvidenceAsync(evidenceId, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo);
    }

#if LIBPLANET_DPOS
    [CommandMethod]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        await evidence.UnjailAsync(cancellationToken);
    }
#endif // LIBPLANET_DPOS

    public static class ListProperties
    {
        [CommandPropertyRequired(DefaultValue = -1)]
        [CommandSummary("The height of the block to get the evidence. default is the tip.")]
        public static long Height { get; set; }

        [CommandPropertySwitch("pending", 'p')]
        [CommandPropertyExclusion(nameof(Height))]
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
