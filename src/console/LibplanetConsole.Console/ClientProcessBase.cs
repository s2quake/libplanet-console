using static LibplanetConsole.Console.ProcessEnvironment;

namespace LibplanetConsole.Console;

public abstract class ClientProcessBase : ProcessBase
{
    public override sealed bool SupportsDotnetRuntime => true;

    public override sealed string FileName => ClientPath;
}
