using System.Collections.Immutable;
using System.Numerics;
using Bencodex;
using Bencodex.Types;
using Google.Protobuf.WellKnownTypes;
using JSSoft.Commands;
using Libplanet.Action;
using Libplanet.Action.Sys;
using Libplanet.Blockchain;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using Validator = Libplanet.Types.Consensus.Validator;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

internal sealed class GenesisCommand : CommandBase
{
    private static readonly BigInteger ValidatorPower = new(1000);

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
            ? AppPrivateKey.Parse(GenesisKey)
            : new AppPrivateKey();
        var validatorKeys = ValidatorKeys.Select(AppPublicKey.Parse).ToArray();
        var validators = validatorKeys
            .Select(item => new Validator((PublicKey)item, ValidatorPower))
            .ToArray();
        var validatorSet = new ValidatorSet(validators: [.. validators]);
        var nonce = 0L;
        var actions = new IAction[]
        {
            new Initialize(
                validatorSet: validatorSet,
                states: ImmutableDictionary.Create<Address, IValue>()),
        };
        var dateTimeOffset = DateTimeOffset.UtcNow;

        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: (PrivateKey)genesisKey,
            genesisHash: null,
            actions: [.. actions.Select(item => item.PlainValue)],
            timestamp: dateTimeOffset);
        var transactions = ImmutableList.Create(transaction);
        var genesisBlock = BlockChain.ProposeGenesisBlock(
            privateKey: (PrivateKey)genesisKey,
            transactions: transactions,
            timestamp: dateTimeOffset);

        var blockDictionary = BlockMarshaler.MarshalBlock(genesisBlock);
        var codec = new Codec();
        var bytes = codec.Encode(blockDictionary);
        File.WriteAllBytes(OutputPath, bytes);
        var info = new
        {
            GenesisKey = ByteUtil.Hex(genesisKey.ToByteArray()),
            ValidatorKeys,
            Timestamp = dateTimeOffset,
        };
        Out.WriteLineAsJson(info);
    }
}
