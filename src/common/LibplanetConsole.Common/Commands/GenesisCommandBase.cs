using Bencodex;
using JSSoft.Commands;
using Libplanet.Common;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.Commands;

[CommandSummary("Creates a genesis.")]
public abstract class GenesisCommandBase : CommandBase
{
    [CommandProperty]
    [AppPrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(ValidatorCount))]
    [AppPublicKeyArray]
    public string[] Validators { get; set; } = [];

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Validators))]
    [NonNegative]
    public int ValidatorCount { get; set; }

    [CommandProperty("date-time")]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandPropertySwitch("raw")]
    public bool IsRaw { get; set; }

    protected override void OnExecute()
    {
        var genesisKey = GenesisKey != string.Empty
            ? AppPrivateKey.Parse(GenesisKey) : new AppPrivateKey();
        var validatorKeys = GetValidators();
        var dateTimeOffset = DateTimeOffset == DateTimeOffset.MinValue
            ? DateTimeOffset.UtcNow : DateTimeOffset;
        var genesisBlock = BlockUtility.CreateGenesisBlock(
            genesisKey, validatorKeys, dateTimeOffset);

        var blockDictionary = BlockMarshaler.MarshalBlock(genesisBlock);
        var codec = new Codec();
        var bytes = codec.Encode(blockDictionary);
        var hex = ByteUtil.Hex(bytes);

        if (IsRaw is true)
        {
            Out.WriteLine(hex);
        }
        else
        {
            var info = new
            {
                GenesisArguments = new
                {
                    GenesisKey = AppPrivateKey.ToString(genesisKey),
                    Validators = validatorKeys,
                    Timestamp = dateTimeOffset,
                },
                Genesis = hex,
            };
            Out.WriteLineAsJson(info);
        }
    }

    private AppPublicKey[] GetValidators()
    {
        if (Validators.Length > 0)
        {
            return [.. Validators.Select(AppPublicKey.Parse)];
        }
        else if (ValidatorCount > 0)
        {
            return Enumerable.Range(0, ValidatorCount)
                .Select(_ => new AppPrivateKey().PublicKey)
                .ToArray();
        }
        else
        {
            return [];
        }
    }
}
