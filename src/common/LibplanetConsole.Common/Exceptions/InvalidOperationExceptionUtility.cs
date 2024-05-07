namespace LibplanetConsole.Common.Exceptions;

public static class InvalidOperationExceptionUtility
{
    public static void ThrowIf(bool condition, string message)
    {
        if (condition == true)
        {
            throw new InvalidOperationException(message);
        }
    }
}
