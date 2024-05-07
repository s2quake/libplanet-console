namespace LibplanetConsole.Common.Serializations;

public readonly record struct GenesisOptionsInfo
{
    public string GenesisKey { get; init; }

    public string[] GenesisValidators { get; init; }

    public string Timestamp { get; init; }

    public static implicit operator GenesisOptions(GenesisOptionsInfo info)
    {
        return new GenesisOptions
        {
            GenesisKey = PrivateKeyUtility.Parse(info.GenesisKey),
            GenesisValidators = [.. info.GenesisValidators.Select(PublicKeyUtility.Parse)],
            Timestamp = info.Timestamp == string.Empty ?
                DateTimeOffset.MinValue : DateTimeOffset.Parse(info.Timestamp),
        };
    }

    public static implicit operator GenesisOptionsInfo(GenesisOptions genesisOptions)
    {
        return new GenesisOptionsInfo
        {
            GenesisKey = PrivateKeyUtility.ToString(genesisOptions.GenesisKey),
            GenesisValidators =
                [.. genesisOptions.GenesisValidators.Select(PublicKeyUtility.ToString)],
            Timestamp = genesisOptions.Timestamp.ToString("O"),
        };
    }
}
