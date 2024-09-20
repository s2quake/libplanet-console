using System.Collections.Immutable;
using System.Numerics;
using System.Security.Cryptography;
using Bencodex;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Sys;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;

namespace LibplanetConsole.Common;

public static class BlockUtility
{
    private static readonly BigInteger ValidatorPower = new(1000);

    private static readonly Codec _codec = new();
    private static readonly HashDigest<SHA256> _emptyRootHash
         = HashDigest<SHA256>.DeriveFrom(_codec.Encode(Null.Value));

    public static Block CreateGenesisBlock(GenesisOptions options)
    {
        var genesisKey = options.GenesisKey;
        var validatorKeys = options.Validators;
        var dateTimeOffset = options.Timestamp;
        return CreateGenesisBlock(
            genesisKey: genesisKey,
            validatorKeys: validatorKeys,
            dateTimeOffset: dateTimeOffset);
    }

    public static Block CreateGenesisBlock(
        AppPrivateKey genesisKey,
        AppPublicKey[] validatorKeys,
        DateTimeOffset dateTimeOffset)
    {
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

        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: (PrivateKey)genesisKey,
            genesisHash: null,
            actions: [.. actions.Select(item => item.PlainValue)],
            timestamp: dateTimeOffset);
        var transactions = ImmutableList.Create(transaction);
        return ProposeGenesisBlock(
            privateKey: (PrivateKey)genesisKey,
            transactions: transactions,
            timestamp: dateTimeOffset);
    }

    public static byte[] CreateGenesis(
        AppPrivateKey genesisKey,
        AppPublicKey[] validators,
        DateTimeOffset dateTimeOffset)
    {
        var genesisBlock = CreateGenesisBlock(genesisKey, validators, dateTimeOffset);
        return SerializeBlock(genesisBlock);
    }

    public static string CreateGenesisString(
        AppPrivateKey genesisKey,
        AppPublicKey[] validators,
        DateTimeOffset dateTimeOffset)
    {
        var genesisBlock = CreateGenesisBlock(genesisKey, validators, dateTimeOffset);
        return ByteUtil.Hex(SerializeBlock(genesisBlock));
    }

    public static byte[] SerializeBlock(Block block)
    {
        var blockDictionary = BlockMarshaler.MarshalBlock(block);
        var codec = new Codec();
        return codec.Encode(blockDictionary);
    }

    public static Block DeserializeBlock(byte[] bytes)
    {
        var codec = new Codec();
        var value = codec.Decode(bytes);
        if (value is not Dictionary blockDictionary)
        {
            throw new InvalidCastException("The given bytes is not a block.");
        }

        return BlockMarshaler.UnmarshalBlock(blockDictionary);
    }

    private static Block ProposeGenesisBlock(
        PrivateKey? privateKey = null,
        HashDigest<SHA256>? stateRootHash = null,
        ImmutableList<Transaction>? transactions = null,
        DateTimeOffset? timestamp = null)
    {
        privateKey ??= new PrivateKey();
        transactions = transactions is { } txs
            ? [.. txs.OrderBy(tx => tx.Id)] : ImmutableList<Transaction>.Empty;

        var content = new BlockContent(
            new BlockMetadata(
                index: 0L,
                timestamp: timestamp ?? DateTimeOffset.UtcNow,
                publicKey: privateKey.PublicKey,
                previousHash: null,
                txHash: BlockContent.DeriveTxHash(transactions),
                lastCommit: null,
                evidenceHash: null),
            transactions: transactions,
            evidence: []);

        PreEvaluationBlock preEval = content.Propose();
        stateRootHash ??= _emptyRootHash;
        return preEval.Sign(privateKey, (HashDigest<SHA256>)stateRootHash);
    }
}
