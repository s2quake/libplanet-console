using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal abstract class NodeProcessBase : ProcessBase
{
    public override sealed bool SupportsDotnetRuntime => true;

    public override sealed string FileName => NodePath;
}
