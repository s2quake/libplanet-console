using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

internal sealed class ApplicationExportProvider(ApplicationContainer container)
        : ExportProvider
{
    private static readonly Type[] ExcludeContractTypes =
    [
        typeof(INodeContent),
        typeof(IClientContent),
    ];

    private static readonly string[] ExcludeContractTypeNames
        = [.. ExcludeContractTypes.Select(AttributedModelServices.GetContractName)];

    private readonly ApplicationContainer _containerContainer = container;

    protected override IEnumerable<Export>? GetExportsCore(
        ImportDefinition definition, AtomicComposition? atomicComposition)
    {
        if (ExcludeContractTypeNames.Contains(definition.ContractName) == true)
        {
            return null;
        }

        return _containerContainer.GetExports(definition);
    }
}
