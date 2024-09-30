using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class NodeRepositoryProcess : NodeProcessBase
{
    public required PrivateKey PrivateKey { get; init; }

    public required AppEndPoint EndPoint { get; init; }

    public string OutputPath { get; set; } = string.Empty;

    public string GenesisPath { get; set; } = string.Empty;

    public override string[] Arguments =>
    [
        "init",
        OutputPath,
        "--private-key",
        PrivateKeyUtility.ToString(PrivateKey),
        "--end-point",
        AppEndPoint.ToString(EndPoint),
        "--genesis-path",
        GenesisPath,
    ];
}
