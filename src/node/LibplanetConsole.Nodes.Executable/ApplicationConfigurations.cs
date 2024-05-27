using System.ComponentModel.Composition;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable;

[Export(typeof(IApplicationConfigurations))]
[method: ImportingConstructor]
internal sealed class ApplicationConfigurations(
    [ImportMany] IEnumerable<IApplicationConfiguration> configurations)
        : ApplicationConfigurationsBase(configurations), IApplicationConfigurations
{
}
