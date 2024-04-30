using System.Net;
using System.Net.Sockets;

namespace LibplanetConsole.Common;

public static class PortUtility
{
    private static readonly List<int> ReservedPortList = [];
    private static readonly List<int> UsedPortList = [];
    private static readonly object LockObject = new();

    public static int GetPort()
    {
        lock (LockObject)
        {
            if (ReservedPortList.Count == 0)
            {
                var v = GetRandomPort();
                while (ReservedPortList.Contains(v) == true || UsedPortList.Contains(v) == true)
                {
                    v = GetRandomPort();
                }

                ReservedPortList.Add(v);
            }

            var port = ReservedPortList[0];
            ReservedPortList.Remove(port);
            UsedPortList.Add(port);
            return port;
        }
    }

    public static void ReleasePort(int port)
    {
        lock (LockObject)
        {
            ReservedPortList.Add(port);
            UsedPortList.Remove(port);
        }
    }

    private static int GetRandomPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
