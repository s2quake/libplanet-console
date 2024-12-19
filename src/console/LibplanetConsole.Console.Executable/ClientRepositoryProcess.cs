using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Executable;

internal sealed class ClientRepositoryProcess : ClientProcessBase
{
    public required PrivateKey PrivateKey { get; init; }

    public required int Port { get; init; }

    public string OutputPath { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;

    public override string[] Arguments
    {
        get
        {
            var argumentList = new List<string>
            {
                "init",
                OutputPath,
                "--private-key",
                PrivateKeyUtility.ToString(PrivateKey),
                "--port",
                $"{Port}",
            };

            if (Alias != string.Empty)
            {
                argumentList.Add("--alias");
                argumentList.Add(Alias);
            }

            return [.. argumentList];
        }
    }
}
