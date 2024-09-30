using System.ComponentModel;
using JSSoft.Commands;
using Libplanet.Common;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Creates a genesis.")]
[CommandExample("genesis --validators \"key1,key2,...\" --timestamp \"2021-01-01T00:00:00Z\"")]
[Category("Tools")]
public sealed class GenesisCommand : CommandBase
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
    [PublicKeyArray]
    public string[] Validators { get; set; } = [];

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Validators))]
    [CommandSummary("The number of validators to create. mutually exclusive with '--validators'.")]
    [NonNegative]
    public int ValidatorCount { get; set; }

    [CommandProperty("timestamp")]
    [CommandSummary("The timestamp of the genesis block. ex) \"2021-01-01T00:00:00Z\"")]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandPropertySwitch("raw")]
    [CommandSummary("If set, this option suppresses the arguments used to create " +
                    "the genesis block.")]
    public bool IsRaw { get; set; }

    [CommandProperty("module-path")]
    [CommandSummary("Indicates the path or the name of the assembly that provides " +
                    "the IActionProvider.")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Indicates the type name of the IActionProvider.")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    public string ActionProviderType { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        var genesisKey = GenesisKey != string.Empty
            ? AppPrivateKey.Parse(GenesisKey) : new AppPrivateKey();
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
                    GenesisKey = AppPrivateKey.ToString(genesisKey),
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
                .Select(_ => new AppPrivateKey().PublicKey)
                .ToArray();
        }
        else
        {
            return [];
        }
    }
}
