namespace LibplanetConsole.Common.Exceptions;

public static class ObjectDisposedExceptionUtility
{
    public static void ThrowIf(bool condition, object instance)
    {
        ObjectDisposedException.ThrowIf(condition, instance);
    }
}
