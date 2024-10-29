using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Executable;

internal sealed class NodeRepositoryProcess : NodeProcessBase
{
    public required PrivateKey PrivateKey { get; init; }

    public required int Port { get; init; }

    public string SeedEndPoint { get; init; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public string GenesisPath { get; set; } = string.Empty;

    public string ActionProviderModulePath { get; set; } = string.Empty;

    public string ActionProviderType { get; set; } = string.Empty;

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
                "--genesis-path",
                GenesisPath,
            };
            if (SeedEndPoint != string.Empty)
            {
                argumentList.Add("--seed-end-point");
                argumentList.Add(SeedEndPoint);
            }

            if (ActionProviderModulePath != string.Empty)
            {
                argumentList.Add("--module-path");
                argumentList.Add(ActionProviderModulePath);
            }

            if (ActionProviderType != string.Empty)
            {
                argumentList.Add("--module-type");
                argumentList.Add(ActionProviderType);
            }

            return [.. argumentList];
        }
    }
}
