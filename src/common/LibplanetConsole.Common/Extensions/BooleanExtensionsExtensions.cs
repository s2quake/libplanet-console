namespace LibplanetConsole.Common.Extensions;

public static class BooleanExtensionsExtensions
{
    public static void ThrowIf(this bool @this, string errorMessage)
    {
        if (@this)
        {
            throw new InvalidOperationException(errorMessage);
        }
    }

    public static void ThrowIfNot(this bool @this, string errorMessage)
    {
        if (@this)
        {
            throw new InvalidOperationException(errorMessage);
        }
    }
}
