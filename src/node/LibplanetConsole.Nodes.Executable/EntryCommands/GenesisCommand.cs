using Bencodex;
using JSSoft.Commands;
using Libplanet.Common;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

internal sealed class GenesisCommand : CommandBase
{
    [CommandPropertyRequired]
    public string OutputPath { get; set; } = string.Empty;

    [CommandProperty]
    [AppPrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty]
    [AppPublicKeyArray]
    public string[] ValidatorKeys { get; set; } = [];

    protected override void OnExecute()
    {
        var genesisKey = GenesisKey != string.Empty
            ? AppPrivateKey.Parse(GenesisKey) : new AppPrivateKey();
        var validatorKeys = ValidatorKeys.Select(AppPublicKey.Parse).ToArray();
        var dateTimeOffset = DateTimeOffset.UtcNow;
        var genesisBlock = BlockUtility.CreateGenesisBlock(
            genesisKey, validatorKeys, dateTimeOffset);

        var blockDictionary = BlockMarshaler.MarshalBlock(genesisBlock);
        var codec = new Codec();
        var bytes = codec.Encode(blockDictionary);
        var hex = ByteUtil.Hex(bytes);
        var directory = Path.GetDirectoryName(OutputPath)
            ?? throw new CommandLineException("Invalid output path.");
        if (Directory.Exists(directory) is false)
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(OutputPath, hex);
        var info = new
        {
            GenesisKey = ByteUtil.Hex(genesisKey.ToByteArray()),
            ValidatorKeys,
            Timestamp = dateTimeOffset,
        };
        Out.WriteLineAsJson(info);
    }
}
