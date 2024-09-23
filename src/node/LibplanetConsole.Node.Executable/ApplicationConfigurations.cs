using System.ComponentModel.Composition;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Executable;

[Export(typeof(IApplicationConfigurations))]
[method: ImportingConstructor]
internal sealed class ApplicationConfigurations(
    [ImportMany] IEnumerable<IApplicationConfiguration> configurations)
        : ApplicationConfigurationsBase(configurations), IApplicationConfigurations
{
}