using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal sealed class NodeSchemaProcess : ProcessBase
{
    public string OutputPath { get; set; } = string.Empty;

    protected override string FileName => IsDotnetRuntime ? DotnetPath : NodePath;

    protected override string[] Arguments
    {
        get
        {
            var argumentList = new List<string>
            {
                "schema",
                "--output",
                OutputPath,
            };

            if (IsDotnetRuntime == true)
            {
                argumentList.Insert(0, NodePath);
            }

            return [.. argumentList];
        }
    }
}
