namespace LibplanetConsole.Common;

public sealed record class GenesisOptions
{
    public AppPrivateKey GenesisKey { get; init; } = new();

    public PublicKey[] Validators { get; init; } = [];

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;

    public string ActionProviderModulePath { get; set; } = string.Empty;

    public string ActionProviderType { get; set; } = string.Empty;
}
