using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Messages;
using Libplanet.Net.Options;
using Libplanet.Net.Transports;
using Nito.AsyncEx;
using Serilog;

namespace LibplanetConsole.NodeServices.Seeds
{
    internal class Seed
    {
        private readonly int _maximumPeersToRefresh;
        private readonly TimeSpan _refreshInterval;
        private readonly TimeSpan _peerLifetime;
        private readonly TimeSpan _pingTimeout;

        private readonly ITransport _transport;
        private readonly CancellationTokenSource _runtimeCancellationTokenSource;
        private readonly ILogger _logger;

        public Seed(
            PrivateKey privateKey,
            string? host,
            int? port,
            IceServer[] iceServers,
            AppProtocolVersion appProtocolVersion,
            IImmutableSet<PublicKey> trustedAppProtocolVersionSigners,
            int maximumPeersToToRefresh,
            TimeSpan refreshInterval,
            TimeSpan peerLifetime,
            TimeSpan pingTimeout)
        {
            _maximumPeersToRefresh = maximumPeersToToRefresh;
            _refreshInterval = refreshInterval;
            _peerLifetime = peerLifetime;
            _pingTimeout = pingTimeout;
            _runtimeCancellationTokenSource = new CancellationTokenSource();
            _transport = NetMQTransport.Create(
                        privateKey,
                        new AppProtocolVersionOptions
                        {
                            AppProtocolVersion = appProtocolVersion,
                            TrustedAppProtocolVersionSigners = trustedAppProtocolVersionSigners,
                        },
                        new HostOptions(host, iceServers, port ?? 0))
                .ConfigureAwait(false).GetAwaiter().GetResult();
            PeerInfos = new ConcurrentDictionary<Address, PeerInfo>();
            _transport.ProcessMessageHandler.Register(ReceiveMessageAsync);

            _logger = Log.ForContext<Seed>();
        }

        public ConcurrentDictionary<Address, PeerInfo> PeerInfos { get; }

        private IEnumerable<BoundPeer> Peers =>
            PeerInfos.Values.Select(peerState => peerState.BoundPeer);

        public async Task StartAsync(
            HashSet<BoundPeer> staticPeers,
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>
            {
                StartTransportAsync(cancellationToken),
                RefreshTableAsync(cancellationToken),
            };
            if (staticPeers.Any())
            {
                tasks.Add(CheckStaticPeersAsync(staticPeers, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        public async Task StopAsync(TimeSpan waitFor)
        {
            await _transport.StopAsync(waitFor);
        }

        private async Task<Task> StartTransportAsync(CancellationToken cancellationToken)
        {
            Task task = _transport.StartAsync(cancellationToken);
            await _transport.WaitForRunningAsync();
            return task;
        }

        private async Task ReceiveMessageAsync(Message message)
        {
            switch (message.Content)
            {
                case FindNeighborsMsg findNeighbors:
                    var neighbors = new NeighborsMsg(Peers);
                    await _transport.ReplyMessageAsync(
                        neighbors,
                        message.Identity,
                        _runtimeCancellationTokenSource.Token);
                    break;

                default:
                    var pong = new PongMsg();
                    await _transport.ReplyMessageAsync(
                        pong,
                        message.Identity,
                        _runtimeCancellationTokenSource.Token);
                    break;
            }

            if (message.Remote is BoundPeer boundPeer)
            {
                AddOrUpdate(boundPeer);
            }
        }

        /// <summary>
        /// Returns added peers.
        /// </summary>
        /// <param name="peers">The list of <see cref="BoundPeer"/>s to add.</param>
        /// <param name="timeout">The timeout of each peer dial.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this
        /// operation should be canceled.</param>
        /// <returns>The list of peers that were successfully added.</returns>
        private async Task<BoundPeer[]> AddPeersAsync(
            BoundPeer[] peers,
            TimeSpan? timeout,
            CancellationToken cancellationToken)
        {
            List<BoundPeer> success = new List<BoundPeer>();
            IEnumerable<Task> tasks = peers.Select(async peer =>
                {
                    try
                    {
                        var ping = new PingMsg();
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        Message? reply = await _transport.SendMessageAsync(
                            peer,
                            ping,
                            timeout,
                            cancellationToken);
                        TimeSpan elapsed = stopwatch.Elapsed;
                        stopwatch.Stop();

                        if (reply.Content is PongMsg)
                        {
                            AddOrUpdate(peer, elapsed);
                            success.Add(peer);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Information(
                            "Operation canceled during {FName}().",
                            nameof(AddPeersAsync),
                            peer);
                        throw;
                    }
                    catch (Exception e)
                    {
                        _logger.Error(
                            e,
                            "Unexpected error occurred during {FName} to {Peer}.",
                            nameof(AddPeersAsync),
                            peer);
                    }
                });

            await tasks.WhenAll();
            return success.ToArray();
        }

        private PeerInfo AddOrUpdate(BoundPeer peer, TimeSpan? latency = null)
        {
            PeerInfo peerInfo;
            peerInfo.BoundPeer = peer;
            peerInfo.LastUpdated = DateTimeOffset.UtcNow;
            peerInfo.Latency = latency;
            return PeerInfos.AddOrUpdate(
                peer.Address,
                peerInfo,
                (address, info) =>
                {
                    peerInfo.Latency = latency ?? info.Latency;
                    return peerInfo;
                });
        }

        private async Task RefreshTableAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // FIXME: Ordered selection of peers may cause some peers does not refreshed
                    // forever.
                    await Task.Delay(_refreshInterval, cancellationToken);
                    BoundPeer[] peersToUpdate = PeerInfos.Values
                        .Where(
                            peerState => DateTimeOffset.UtcNow - peerState.LastUpdated >
                                         _peerLifetime)
                        .Select(state => state.BoundPeer)
                        .Take(_maximumPeersToRefresh)
                        .ToArray();
                    _logger.Debug(
                        "Refreshing peers in table. (Total: {Total}, Candidate: {Candidate})",
                        Peers.Count(),
                        peersToUpdate.Length);
                    if (peersToUpdate.Any())
                    {
                        var updated = await AddPeersAsync(
                            peersToUpdate.ToArray(),
                            _pingTimeout,
                            cancellationToken);

                        // Remove stale peers.
                        // TODO: Should give grace to peers before removing.
                        foreach (BoundPeer rm in peersToUpdate.Where(p => !updated.Contains(p)))
                        {
                            PeerInfos.TryRemove(rm.Address, out _);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Information(
                        "Operation canceled during {FName}().",
                        nameof(RefreshTableAsync));
                    throw;
                }
                catch (Exception e)
                {
                    Log.Warning(
                        e,
                        "Unexpected exception occurred during {FName}().",
                        nameof(RefreshTableAsync));
                }
            }
        }

        private async Task CheckStaticPeersAsync(
            IEnumerable<BoundPeer> peers,
            CancellationToken cancellationToken)
        {
            var boundPeers = peers as BoundPeer[] ?? peers.ToArray();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    Log.Warning("Checking static peers. {@Peers}", boundPeers);
                    var peersToAdd = boundPeers.Where(peer => !Peers.Contains(peer)).ToArray();
                    if (peersToAdd.Any())
                    {
                        Log.Warning("Some of peers are not in routing table. {@Peers}", peersToAdd);
                        await AddPeersAsync(
                            peersToAdd,
                            _pingTimeout,
                            cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Information(
                        "Operation canceled during {FName}().",
                        nameof(CheckStaticPeersAsync));
                    throw;
                }
                catch (Exception e)
                {
                    Log.Warning(
                        e,
                        "Unexpected exception occurred during {FName}().",
                        nameof(CheckStaticPeersAsync));
                }
            }
        }
    }
}
