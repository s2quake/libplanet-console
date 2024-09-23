namespace LibplanetConsole.Console.Tests;

public sealed class TestProcess(string filename, params string[] arguments) : ProcessBase
{
    public override string FileName { get; } = filename;

    public override string[] Arguments { get; } = arguments;
}
