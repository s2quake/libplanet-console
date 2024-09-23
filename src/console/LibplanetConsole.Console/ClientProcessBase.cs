using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal abstract class ClientProcessBase : ProcessBase
{
    public override sealed bool SupportsDotnetRuntime => true;

    public override sealed string FileName => ClientPath;
}
