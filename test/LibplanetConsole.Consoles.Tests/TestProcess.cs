namespace LibplanetConsole.Consoles.Tests;

public sealed class TestProcess(string filename, params string[] arguments) : ProcessBase
{
    public override string FileName { get; } = filename;

    public override string[] Arguments { get; } = arguments;
}
