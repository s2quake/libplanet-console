using System.ComponentModel;
using JSSoft.Commands;
using Libplanet.Types.Evidence;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Evidence.Commands;

[CommandSummary("Provides evidence-related commands")]
[Category("Evidence")]
internal sealed class NodeEvidenceCommand(IServiceProvider serviceProvider, NodeCommand nodeCommand)
    : NodeCommandMethodBase(serviceProvider, nodeCommand, "evidence")
{
    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Adds a new evidence")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        var evidence = node.GetRequiredKeyedService<INodeEvidence>(INode.Key);
        var evidenceInfo = await evidence.AddEvidenceAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandMethodStaticProperty(typeof(ViolateProperties))]
    [CommandSummary("Raises infraction")]
    public async Task ViolateAsync(
        CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        var evidence = node.GetRequiredKeyedService<INodeEvidence>(INode.Key);
        if (ViolateProperties.DuplicateVote is true)
        {
            await evidence.ViolateAsync("duplicate_vote", cancellationToken);
        }
        else
        {
            throw new InvalidOperationException(
                "The type of the evidence to violate is not specified.");
        }
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandMethodStaticProperty(typeof(ListProperties))]
    [CommandSummary("Gets the evidence list")]
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        var evidence = node.GetRequiredKeyedService<INodeEvidence>(INode.Key);
        var height = ListProperties.Height;
        var isPending = ListProperties.IsPending;
        var evidenceInfos = isPending is true ?
            await evidence.GetPendingEvidenceAsync(cancellationToken) :
            await evidence.GetEvidenceAsync(height, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfos, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandMethodStaticProperty(typeof(GetProperties))]
    [CommandSummary("Gets the evidence")]
    public async Task GetAsync(string evidenceId, CancellationToken cancellationToken)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        var evidence = node.GetRequiredKeyedService<INodeEvidence>(INode.Key);
        var isPending = GetProperties.IsPending;
        var id = EvidenceId.Parse(evidenceId);
        var evidenceInfo = isPending is true ?
            await evidence.GetPendingEvidenceAsync(id, cancellationToken) :
            await evidence.GetEvidenceAsync(id, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo, cancellationToken);
    }

    public static class ViolateProperties
    {
        [CommandPropertySwitch]
        [CommandSummary("Specifies the type of the evidence to violate")]
        public static bool DuplicateVote { get; set; }
    }

    public static class ListProperties
    {
        [CommandPropertyRequired(DefaultValue = -1)]
        [CommandSummary("Specifies the height of the block to get the evidence. " +
                        "Default is the tip")]
        public static long Height { get; set; }

        [CommandPropertySwitch("pending", 'p')]
        [CommandPropertyExclusion(nameof(Height))]
        [CommandSummary("Specifies whether to get pending evidence.")]
        public static bool IsPending { get; set; }
    }

    public static class GetProperties
    {
        [CommandPropertySwitch("pending", 'p')]
        [CommandSummary("Specifies whether to get pending evidence")]
        public static bool IsPending { get; set; }
    }
}
