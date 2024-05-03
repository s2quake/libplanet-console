using System;
using Libplanet.Net;

namespace LibplanetConsole.NodeServices.Seeds
{
    internal struct PeerInfo
    {
        public BoundPeer BoundPeer;
        public DateTimeOffset LastUpdated;
        public TimeSpan? Latency;
    }
}
