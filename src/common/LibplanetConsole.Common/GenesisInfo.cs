using System.Globalization;

namespace LibplanetConsole.Common;

public readonly record struct GenesisInfo
{
    public string GenesisKey { get; init; }

    public string[] GenesisValidators { get; init; }

    public string Timestamp { get; init; }

    public static implicit operator GenesisOptions(GenesisInfo info) => new()
    {
        GenesisKey = AppPrivateKey.Parse(info.GenesisKey),
        GenesisValidators = [.. info.GenesisValidators.Select(AppPublicKey.Parse)],
        Timestamp = info.Timestamp == string.Empty
            ? DateTimeOffset.MinValue
            : DateTimeOffset.ParseExact(info.Timestamp, "O", CultureInfo.CurrentCulture),
    };

    public static implicit operator GenesisInfo(GenesisOptions genesisOptions) => new()
    {
        GenesisKey = AppPrivateKey.ToString(genesisOptions.GenesisKey),
        GenesisValidators
            = [.. genesisOptions.GenesisValidators.Select(item => item.ToString())],
        Timestamp = genesisOptions.Timestamp.ToString("O"),
    };

    public GenesisInfo Encrypt(AppPublicKey publicKey) => this with
    {
        GenesisKey = publicKey.Encrypt(GenesisKey),
        GenesisValidators = [.. GenesisValidators.Select(publicKey.Encrypt)],
    };

    public GenesisInfo Decrypt(AppPrivateKey privateKey) => this with
    {
        GenesisKey = Decrypt(privateKey, GenesisKey),
        GenesisValidators = [.. GenesisValidators.Select(item => Decrypt(privateKey, item))],
    };

    private static string Decrypt(AppPrivateKey privateKey, string text)
        => privateKey.Decrypt<string>(text);
}
