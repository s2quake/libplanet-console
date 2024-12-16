namespace LibplanetConsole.Node.Executable.Extensions;

internal static class GenesisOptionsExtensions
{
    public static GenesisOptions EnsureActionProviderType(
        this GenesisOptions @this, Type fallbackType)
    {
        if (@this.ActionProviderModulePath == string.Empty
            && @this.ActionProviderType == string.Empty)
        {
            @this.ActionProviderType = fallbackType.AssemblyQualifiedName!;
        }

        return @this;
    }
}
