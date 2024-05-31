using System.ComponentModel.Composition;
using System.Numerics;
using System.Reflection;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Consensus;
using Libplanet.Net.Messages;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Evidences;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class DuplicateVotePerpetrator(INode node) : IApplicationService, IAsyncDisposable
{
    private readonly PrivateKey _privateKey = PrivateKeyUtility.Parse(
        "b52e619962057e397f47efcb009ce45341f84cb86f425cd081cb64f1f1c1b220"
    );

    private readonly INode _node = node;
    private Task _infractionTask = Task.CompletedTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        _node.Started += Node_Started;
        _node.Stopped += Node_Stopped;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _node.Started -= Node_Started;
        _node.Stopped -= Node_Stopped;
        await _infractionTask;
    }

    private static ConsensusReactor GetConsensusReactor(Swarm swarm)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var propertyInfo = typeof(Swarm).GetProperty(nameof(ConsensusReactor), bindingFlags) ??
            throw new InvalidOperationException("ConsensusReactor property not found.");
        if (propertyInfo.GetValue(swarm) is ConsensusReactor consensusReactor)
        {
            return consensusReactor;
        }

        throw new InvalidOperationException("ConsensusReactor value cannot be null.");
    }

    private static ConsensusContext GetConsensusContext(ConsensusReactor consensusReactor)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(ConsensusReactor);
        var propertyInfo = type.GetProperty(nameof(ConsensusContext), bindingFlags) ??
            throw new InvalidOperationException("ConsensusContext property not found.");
        if (propertyInfo.GetValue(consensusReactor) is ConsensusContext consensusContext)
        {
            return consensusContext;
        }

        throw new InvalidOperationException("ConsensusContext value cannot be null.");
    }

    private static Context GetContext(ConsensusContext consensusContext, long height)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(ConsensusContext);
        var propertyInfo = type.GetProperty(nameof(Context), bindingFlags) ??
            throw new InvalidOperationException("Context property not found.");
        if (propertyInfo.GetValue(consensusContext) is Dictionary<long, Context> contexts)
        {
            return contexts[height];
        }

        throw new InvalidOperationException("Context value cannot be null.");
    }

    private static void InvokeInfraction(
        Context context, PrivateKey privateKey, long height, int round)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var methodName = "PublishMessage";
        var methodInfo = context.GetType().GetMethod(methodName, bindingFlags) ??
            throw new InvalidOperationException(
                $"{nameof(Context)} does not have a method named {methodName}.");
        var vote = MakeRandomVote(privateKey, height, round, VoteFlag.PreVote);
        var args = new object[] { new ConsensusPreVoteMsg(vote) };
        methodInfo.Invoke(context, args);
    }

    private static Vote MakeRandomVote(
            PrivateKey privateKey, long height, int round, VoteFlag flag)
    {
        if (flag == VoteFlag.Null || flag == VoteFlag.Unknown)
        {
            throw new ArgumentException(
                $"{nameof(flag)} must be either {VoteFlag.PreVote} or {VoteFlag.PreCommit}" +
                $"to create a valid signed vote.");
        }

        var hash = new BlockHash(GetRandomBytes(BlockHash.Size));
        var voteMetadata = new VoteMetadata(
            height,
            round,
            hash,
            DateTimeOffset.UtcNow,
            privateKey.PublicKey,
            BigInteger.One,
            flag);

        return voteMetadata.Sign(privateKey);

        static byte[] GetRandomBytes(int size)
        {
            var bytes = new byte[size];
            var random = new System.Random();
            random.NextBytes(bytes);

            return bytes;
        }
    }

    private static async Task InfractionAsync(
        Swarm swarm, PrivateKey privateKey, CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested != true)
        {
            var consensusReactor = GetConsensusReactor(swarm);
            var consensusContext = GetConsensusContext(consensusReactor);
            await WaitUntilStepAsync(consensusContext, ConsensusStep.PreVote, cancellationToken);

            if (Random.Shared.Next() % 3 == 0)
            {
                var height = consensusContext.Height;
                var round = (int)consensusContext.Round;
                var context = GetContext(consensusContext, height);
                InvokeInfraction(context, privateKey, height, round);
            }

            try
            {
                await Task.Delay(1000, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private static async Task WaitUntilStepAsync(
        ConsensusContext consensusContext,
        ConsensusStep consensusStep,
        CancellationToken cancellationToken)
    {
        while (consensusContext.Step != consensusStep)
        {
            try
            {
                await Task.Delay(1, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private void Node_Started(object? sender, EventArgs e)
    {
        if (_node.Address == _privateKey.Address)
        {
            _cancellationTokenSource = new();
            _infractionTask = InfractionAsync(
                _node.Swarm, _privateKey, _cancellationTokenSource.Token);
        }
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _infractionTask = Task.CompletedTask;
    }
}
