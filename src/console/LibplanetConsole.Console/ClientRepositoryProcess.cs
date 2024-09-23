using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class ClientRepositoryProcess : ClientProcessBase
{
    public required AppPrivateKey PrivateKey { get; init; }

    public required AppEndPoint EndPoint { get; init; }

    public string OutputPath { get; set; } = string.Empty;

    public override string[] Arguments =>
    [
       "init",
        OutputPath,
        "--private-key",
        AppPrivateKey.ToString(PrivateKey),
        "--end-point",
        AppEndPoint.ToString(EndPoint),
    ];
}
