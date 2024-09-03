using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal sealed class ClientSchemaProcess : ProcessBase
{
    public string OutputPath { get; set; } = string.Empty;

    protected override string FileName => IsDotnetRuntime ? DotnetPath : ClientPath;

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
                argumentList.Insert(0, ClientPath);
            }

            return [.. argumentList];
        }
    }
}
