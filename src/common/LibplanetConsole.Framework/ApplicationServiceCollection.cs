using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Framework;

public sealed class ApplicationServiceCollection : ServiceCollection
{
    public ApplicationServiceCollection()
        : this(new())
    {
    }

    public ApplicationServiceCollection(ApplicationSettingsCollection settingsCollection)
    {
        foreach (var settings in settingsCollection)
        {
            this.AddSingleton(settings);
        }
    }
}
