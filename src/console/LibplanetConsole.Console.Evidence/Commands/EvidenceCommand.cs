using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Evidence.Commands;

[CommandSummary("Provides evidence-related commands.")]
[Category("Evidence")]
internal sealed class EvidenceCommand(INodeCollection nodes)
    : CommandMethodBase
{
    [CommandMethod]
    public async Task NewAsync(
        string nodeAddress = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidence = node.GetRequiredService<IEvidence>();
        var evidenceInfo = await evidence.AddEvidenceAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo);
    }

    [CommandMethod]
    public async Task RaiseAsync(
        CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidence = node.GetRequiredService<IEvidence>();
        await evidence.ViolateAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task ListAsync(long height = -1, CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidence = node.GetRequiredService<IEvidence>();
        var evidenceInfos = await evidence.GetEvidenceAsync(height, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfos);
    }

#if LIBPLANET_DPOS
    [CommandMethod]
    public async Task UnjailAsync(
        CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidence = node.GetService<IEvidenceContent>();
        await evidence.UnjailAsync(cancellationToken);
    }
#endif // LIBPLANET_DPOS
}
