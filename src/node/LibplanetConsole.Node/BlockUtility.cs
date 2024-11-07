using System.Collections.Immutable;
using System.Security.Cryptography;

namespace LibplanetConsole.Node;

public static partial class BlockUtility
{
    public static Block CreateGenesisBlock(
        GenesisOptions genesisOptions)
    {
        var genesisKey = genesisOptions.GenesisKey;
        var validators = genesisOptions.Validators;
        var timestamp = genesisOptions.Timestamp;
        var actionLoaderProvider = ModuleLoader.LoadActionLoader(
            genesisOptions.ActionProviderModulePath,
            genesisOptions.ActionProviderType);
        var actions = actionLoaderProvider.GetGenesisActions(genesisKey, validators);
        var genesisBlock = CreateGenesisBlock(genesisKey, timestamp, actions);

        return genesisBlock;
    }

    private static Block CreateGenesisBlock(
        PrivateKey genesisKey,
        DateTimeOffset dateTimeOffset,
        IAction[] actions)
    {
        var nonce = 0L;
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: genesisKey,
            genesisHash: null,
            actions: [.. actions.Select(item => item.PlainValue)],
            timestamp: dateTimeOffset);
        var transactions = ImmutableList.Create(transaction);
        return ProposeGenesisBlock(
            privateKey: genesisKey,
            transactions: transactions,
            timestamp: dateTimeOffset);
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
        stateRootHash ??= HashDigest<SHA256>.DeriveFrom(_codec.Encode(Null.Value));
        return preEval.Sign(privateKey, (HashDigest<SHA256>)stateRootHash);
    }
}
