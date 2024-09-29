using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class NodeGenesisProcess : NodeProcessBase
{
    public required GenesisOptions GenesisOptions { get; init; }

    public override string[] Arguments =>
    [
        "genesis",
        "--genesis-key",
        AppPrivateKey.ToString(GenesisOptions.GenesisKey),
        "--validators",
        string.Join(",", GenesisOptions.Validators.Select(item => item.ToString())),
        "--timestamp",
        GenesisOptions.Timestamp.ToString(),
        "--module-path",
        GenesisOptions.ActionProviderModulePath,
        "--module-type",
        GenesisOptions.ActionProviderType,
        "--raw",
    ];
}
