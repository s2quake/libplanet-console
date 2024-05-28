using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Evidences.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides evidence-related commands.")]
[Category("Evidences")]
[method: ImportingConstructor]
internal sealed class EvidenceCommand(INodeCollection nodes)
    : CommandMethodBase
{
    [CommandMethod]
    public async Task NewAsync(
        string nodeAddress = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidenceContent = node.GetService<IEvidenceContent>();
        var evidenceInfo = await evidenceContent.AddEvidenceAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfo);
    }

    [CommandMethod]
    public async Task ListAsync(long height = -1, CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidenceContent = node.GetService<IEvidenceContent>();
        var evidenceInfos = await evidenceContent.GetEvidencesAsync(height, cancellationToken);
        await Out.WriteLineAsJsonAsync(evidenceInfos);
    }

    [CommandMethod]
    public async Task UnjailAsync(
        CancellationToken cancellationToken = default)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        var evidenceContent = node.GetService<IEvidenceContent>();
        await evidenceContent.UnjailAsync(cancellationToken);
    }
}
