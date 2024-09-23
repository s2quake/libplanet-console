namespace LibplanetConsole.Console;

public class ProcessExecutionException : Exception
{
    public ProcessExecutionException(string error, int exitCode)
        : base(GetMessage(error, exitCode))
    {
        ExitCode = exitCode;
    }

    public ProcessExecutionException(string error, int exitCode, Exception innerException)
        : base(GetMessage(error, exitCode), innerException)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }

    public string CommandLine { get; set; } = string.Empty;

    private static string GetMessage(string error, int exitCode)
    {
        return $"The process exited with code {exitCode} and error below.\n{error}";
    }
}
