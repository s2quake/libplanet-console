using System.Diagnostics;
using Libplanet.Net.Messages;
using Libplanet.Net.Transports;

namespace LibplanetConsole.Seed;

public sealed class Peer
{
    private readonly ITransport _transport;

    internal Peer(ITransport transport, BoundPeer appPeer)
    {
        _transport = transport;
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

    internal async Task<bool> PingAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            var pingMsg = new PingMsg();
            var stopwatch = Stopwatch.StartNew();
            var replyMessage = await _transport.SendMessageAsync(
                BoundPeer,
                pingMsg,
                timeout,
                cancellationToken);
            var latency = Stopwatch.GetElapsedTime(stopwatch.ElapsedTicks);

            if (replyMessage.Content is PongMsg)
            {
                Latency = latency;
                return true;
            }
        }
        catch
        {
            // Ignore
        }

        Latency = TimeSpan.MinValue;
        return false;
    }
}
