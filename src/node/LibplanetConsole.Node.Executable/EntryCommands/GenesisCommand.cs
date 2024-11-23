using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Creates a genesis")]
[CommandExample("genesis --validators \"key1,key2,...\" --timestamp \"2021-01-01T00:00:00Z\"")]
[Category("Tools")]
public sealed class GenesisCommand : CommandBase
{
    [CommandProperty]
    [CommandSummary("Specifies the private key of the genesis block")]
    [PrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(ValidatorCount))]
    [CommandSummary("Specifies Tthe public keys of the validators")]
    [PublicKeyArray]
    public string[] Validators { get; set; } = [];

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Validators))]
    [CommandSummary("Specifies the number of validators to create")]
    [NonNegative]
    public int ValidatorCount { get; set; }

    [CommandProperty("timestamp")]
    [CommandSummary("Specifies the timestamp of the genesis block")]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandPropertySwitch("raw")]
    [CommandSummary("If set, only the genesis is displayed")]
    public bool IsRaw { get; set; }

    [CommandProperty("module-path")]
    [CommandSummary("Specifies the path or the name of the assembly that provides " +
                    "the IActionProvider.")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Specifies the type name of the IActionProvider")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    public string ActionProviderType { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        var genesisKey = PrivateKeyUtility.ParseOrRandom(GenesisKey);
        var validatorKeys = GetValidators();
        var dateTimeOffset = DateTimeOffset == DateTimeOffset.MinValue
            ? DateTimeOffset.UtcNow : DateTimeOffset;
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = genesisKey,
            Validators = validatorKeys,
            Timestamp = dateTimeOffset,
            ActionProviderModulePath = ActionProviderModulePath,
            ActionProviderType = ActionProviderType,
        };
        var genesisBlock = BlockUtility.CreateGenesisBlock(genesisOptions);
        var genesis = BlockUtility.SerializeBlock(genesisBlock);
        var hex = ByteUtil.Hex(genesis);

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
                    GenesisKey = PrivateKeyUtility.ToString(genesisKey),
                    Validators = validatorKeys,
                    Timestamp = dateTimeOffset,
                },
                Genesis = hex,
            };
            Out.WriteLineAsJson(info);
        }
    }

    private PublicKey[] GetValidators()
    {
        if (Validators.Length > 0)
        {
            return [.. Validators.Select(PublicKey.FromHex)];
        }
        else if (ValidatorCount > 0)
        {
            return Enumerable.Range(0, ValidatorCount)
                .Select(_ => new PrivateKey().PublicKey)
                .ToArray();
        }
        else
        {
            return [];
        }
    }
}
