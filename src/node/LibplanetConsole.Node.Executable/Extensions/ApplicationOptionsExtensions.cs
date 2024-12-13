namespace LibplanetConsole.Node.Executable.Extensions;

internal static class ApplicationOptionsExtensions
{
    public static void EnsureActionProviderType(
        this ApplicationOptions @this, Type fallbackType)
    {
        if (@this.ActionProviderModulePath == string.Empty
            && @this.ActionProviderType == string.Empty)
        {
            @this.ActionProviderType = fallbackType.AssemblyQualifiedName!;
        }
    }
}
