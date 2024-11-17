using System.Diagnostics;
using Libplanet.Net.Messages;
using Libplanet.Net.Transports;
using Serilog;

namespace LibplanetConsole.Seed;

public sealed class Peer
{
    private readonly ILogger _logger = Log.ForContext<SeedNode>();

    internal Peer(BoundPeer appPeer)
    {
        BoundPeer = appPeer;
    }

    public Address Address => BoundPeer.Address;

    public BoundPeer BoundPeer { get; }

    public DateTimeOffset LastUpdated { get; private set; }

    public DateTimeOffset LifeTime { get; private set; }

    public TimeSpan LifeTimeSpan { get; init; } = TimeSpan.FromSeconds(120);

    public TimeSpan Latency { get; private set; } = TimeSpan.MinValue;

    public bool IsAlive => DateTimeOffset.UtcNow < LifeTime;

    internal void Update()
    {
        LastUpdated = DateTimeOffset.UtcNow;
        LifeTime = LastUpdated + LifeTimeSpan;
    }

    internal async Task<bool> PingAsync(
        ITransport transport, TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            var pingMsg = new PingMsg();
            var stopwatch = Stopwatch.StartNew();
            var replyMessage = await transport.SendMessageAsync(
                BoundPeer, pingMsg, timeout, cancellationToken);
            var latency = Stopwatch.GetElapsedTime(stopwatch.ElapsedTicks);

            if (replyMessage.Content is PongMsg)
            {
                Latency = latency;
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to ping a peer: {BoundPeer}", BoundPeer);
        }

        Latency = TimeSpan.MinValue;
        return false;
    }
}
