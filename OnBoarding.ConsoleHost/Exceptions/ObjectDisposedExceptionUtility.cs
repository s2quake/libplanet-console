namespace OnBoarding.ConsoleHost.Exceptions;

static class ObjectDisposedExceptionUtility
{
    public static void ThrowIf(bool condition, object instance)
    {
        ObjectDisposedException.ThrowIf(condition, instance);
    }
}
