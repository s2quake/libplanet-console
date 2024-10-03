using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Framework;

public sealed class ApplicationServiceCollection : ServiceCollection
{
    public ApplicationServiceCollection()
    {
    }

    public ApplicationServiceCollection(ApplicationSettingsCollection settingsCollection)
    {
        foreach (var settings in settingsCollection)
        {
            this.AddSingleton(settings);
        }
    }

    public static IEnumerable<Assembly> GetAssemblies()
        => GetAssemblies(Assembly.GetEntryAssembly()!);

    public static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
    {
        var directory = Path.GetDirectoryName(assembly.Location)!;
        var files = Directory.GetFiles(directory, "LibplanetConsole.*.dll");
        string[] paths =
        [
            assembly.Location,
            .. files,
        ];
        return [.. paths.Distinct().Order().Select(Assembly.LoadFrom)];
    }
}
