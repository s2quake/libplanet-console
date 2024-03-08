namespace LibplanetConsole.Executable.Exceptions;

static class ArgumentExceptionUtility
{
    public static void ThrowIf(bool condition, string message, string paramName)
    {
        if (condition == true)
        {
            throw new ArgumentException(message, paramName);
        }
    }
}
