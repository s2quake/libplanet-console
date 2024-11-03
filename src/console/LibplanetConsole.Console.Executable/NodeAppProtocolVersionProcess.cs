using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Executable;

internal sealed class NodeAppProtocolVersionProcess : NodeProcessBase
{
    public required PrivateKey PrivateKey { get; init; }

    public required int Version { get; init; } = 1;

    public string Extra { get; init; } = string.Empty;

    public override string[] Arguments
    {
        get
        {
            var argumentList = new List<string>
            {
                "apv",
                PrivateKeyUtility.ToString(PrivateKey),
                $"{Version}",
                "--raw",
            };

            if (Extra != string.Empty)
            {
                argumentList.Add("--extra");
                argumentList.Add(Extra);
            }

            return [.. argumentList];
        }
    }
}
