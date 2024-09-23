using System.Reflection;

namespace LibplanetConsole.Console.Tests.Extensions;

internal static class AssemblyExtensions
{
    public static string GetManifestResourceString(this Assembly @this, string name)
    {
        using var codeStream = @this.GetManifestResourceStream(name)
            ?? throw new FileNotFoundException($"Resource '{name}' not found.");
        using var reader = new StreamReader(codeStream);
        return reader.ReadToEnd();
    }
}
