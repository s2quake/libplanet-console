using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost;

internal sealed class ApplicationExportProvider(CompositionContainer compositionContainer)
        : ExportProvider
{
    private static readonly Type[] ExcludeContractTypes =
    [
        typeof(INodeContent),
        typeof(IClientContent),
    ];

    private static readonly string[] ExcludeContractTypeNames
        = [.. ExcludeContractTypes.Select(AttributedModelServices.GetContractName)];

    private readonly CompositionContainer _compositionContainer = compositionContainer;

    protected override IEnumerable<Export>? GetExportsCore(
        ImportDefinition definition, AtomicComposition? atomicComposition)
    {
        if (ExcludeContractTypeNames.Contains(definition.ContractName) == true)
        {
            return null;
        }

        return _compositionContainer.GetExports(definition);
    }
}
