using System.Reflection;

namespace LibplanetConsole.Framework;

public static class ApplicationServiceCollection
{
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
