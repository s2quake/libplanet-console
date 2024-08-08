using System.Diagnostics;
using Libplanet.Net.Messages;
using Libplanet.Net.Transports;
using LibplanetConsole.Common;
using static LibplanetConsole.Seeds.PeerUtility;

namespace LibplanetConsole.Seeds;

public sealed class Peer
{
    private readonly ITransport _transport;

    internal Peer(ITransport transport, AppPeer appPeer)
    {
        _transport = transport;
        AppPeer = appPeer;
    }

    public AppAddress Address => AppPeer.Address;

    public AppPeer AppPeer { get; }

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
                ToBoundPeer(AppPeer),
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
