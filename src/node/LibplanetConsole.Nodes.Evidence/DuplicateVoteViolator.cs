using System.Numerics;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Consensus;
using Libplanet.Net.Messages;
using Libplanet.Types.Blocks;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes.Evidence.Extensions;
using Serilog;

namespace LibplanetConsole.Nodes.Evidence;

internal sealed class DuplicateVoteViolator(INode node) : IAsyncDisposable
{
    private const int Timeout = 5000;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ILogger _logger = node.GetService<ILogger>();

    public bool IsRunning { get; private set; }

    public async Task ViolateAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == true)
        {
            throw new InvalidOperationException("The perpetrator is already running.");
        }

        using var scope = new RunningScope(this);
        var swarm = node.GetService<Swarm>();
        var consensusReactor = swarm.GetConsensusReactor();
        var consensusContext = consensusReactor.GetConsensusContext();
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            _cancellationTokenSource.Token, cancellationToken);
        cancellationTokenSource.CancelAfter(Timeout);
        await consensusContext.WaitUntilPreVoteAsync(cancellationTokenSource.Token);
        InvokeViolation(consensusContext);
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
    }

    private static Vote MakeRandomVote(
        INode node, long height, int round, BigInteger power)
    {
        var hash = new BlockHash(GetRandomBytes(BlockHash.Size));
        var publicKey = (PublicKey)node.PublicKey;
        var voteMetadata = new VoteMetadata(
            height,
            round,
            hash,
            DateTimeOffset.UtcNow,
            publicKey,
            power,
            VoteFlag.PreVote);
        var signature = node.Sign(voteMetadata.Bencoded);

        return new Vote(voteMetadata, [.. signature]);

        static byte[] GetRandomBytes(int size)
        {
            var bytes = new byte[size];
            var random = new Random();
            random.NextBytes(bytes);

            return bytes;
        }
    }

    private void InvokeViolation(ConsensusContext consensusContext)
    {
        var height = consensusContext.Height;
        var round = (int)consensusContext.Round;
        var context = consensusContext.GetContext();
        var validatorSet = context.GetValidatorSet();
        var publicKey = (PublicKey)node.PublicKey;
        var validator = validatorSet.Validators.First(item => item.PublicKey == publicKey);
        var power = validator.Power;
        var vote = MakeRandomVote(node, height, round, power);
        var message = new ConsensusPreVoteMsg(vote);
        context.PublishMessage(message);
        _logger.Debug("Violation invoked: {Height}, {Round}", height, round);
    }

    private sealed class RunningScope : IDisposable
    {
        private readonly DuplicateVoteViolator _obj;

        public RunningScope(DuplicateVoteViolator obj)
        {
            _obj = obj;
            _obj.IsRunning = true;
        }

        public void Dispose()
        {
            _obj.IsRunning = false;
        }
    }
}
