using System.Globalization;
using Libplanet.Crypto;

namespace LibplanetConsole.Common.Serializations;

public readonly record struct GenesisInfo
{
    public string GenesisKey { get; init; }

    public string[] GenesisValidators { get; init; }

    public string Timestamp { get; init; }

    public static implicit operator GenesisOptions(GenesisInfo info)
    {
        return new GenesisOptions
        {
            GenesisKey = PrivateKeyUtility.Parse(info.GenesisKey),
            GenesisValidators = [.. info.GenesisValidators.Select(PublicKeyUtility.Parse)],
            Timestamp = info.Timestamp == string.Empty
                ? DateTimeOffset.MinValue
                : DateTimeOffset.ParseExact(info.Timestamp, "O", CultureInfo.CurrentCulture),
        };
    }

    public static implicit operator GenesisInfo(GenesisOptions genesisOptions)
    {
        return new GenesisInfo
        {
            GenesisKey = PrivateKeyUtility.ToString(genesisOptions.GenesisKey),
            GenesisValidators =
                [.. genesisOptions.GenesisValidators.Select(PublicKeyUtility.ToString)],
            Timestamp = genesisOptions.Timestamp.ToString("O"),
        };
    }

    public GenesisInfo Encrypt(PublicKey publicKey)
    {
        return this with
        {
            GenesisKey = PublicKeyUtility.Encrypt(publicKey, GenesisKey),
            GenesisValidators
                = [.. GenesisValidators.Select(item => PublicKeyUtility.Encrypt(publicKey, item))],
        };
    }

    public GenesisInfo Decrypt(PrivateKey privateKey)
    {
        return this with
        {
            GenesisKey = Decrypt(privateKey, GenesisKey),
            GenesisValidators
                = [.. GenesisValidators.Select(item => Decrypt(privateKey, item))],
        };

        static string Decrypt(PrivateKey privateKey, string text)
            => PrivateKeyUtility.Decrypt<string>(privateKey, text);
    }
}
