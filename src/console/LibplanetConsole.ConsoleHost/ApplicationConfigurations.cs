using System.ComponentModel.Composition;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.ConsoleHost;

[Export(typeof(IApplicationConfigurations))]
[method: ImportingConstructor]
internal sealed class ApplicationConfigurations(
    [ImportMany] IEnumerable<IApplicationConfiguration> configurations)
        : ApplicationConfigurationsBase(configurations), IApplicationConfigurations
{
}
