using Bencodex;
using JSSoft.Commands;
using Libplanet.Common;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.Commands;

[CommandSummary("Creates a genesis.")]
[CommandExample("genesis --validators \"key1,key2,...\" --date-time \"2021-01-01T00:00:00Z\"")]
public abstract class GenesisCommandBase : CommandBase
{
    [CommandProperty]
    [CommandSummary("The private key of the genesis block. " +
                    "if omitted, a random private key is used.")]
    [AppPrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(ValidatorCount))]
    [CommandSummary("The public keys of the validators. mutually exclusive with " +
                    "'--validator-count'.")]
    [AppPublicKeyArray]
    public string[] Validators { get; set; } = [];

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Validators))]
    [CommandSummary("The number of validators to create. mutually exclusive with '--validators'.")]
    [NonNegative]
    public int ValidatorCount { get; set; }

    [CommandProperty("date-time")]
    [CommandSummary("The timestamp of the genesis block. ex) \"2021-01-01T00:00:00Z\"")]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandPropertySwitch("raw")]
    [CommandSummary("If set, this option suppresses the arguments used to create " +
                    "the genesis block.")]
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
