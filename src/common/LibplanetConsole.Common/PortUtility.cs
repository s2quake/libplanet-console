using System.Net.Sockets;

namespace LibplanetConsole.Common;

public static class PortUtility
{
    private const int TryCount = 10;
    private static readonly List<int> PortList = [];
    private static readonly object LockObject = new();

    public static int NextPort()
    {
        lock (LockObject)
        {
            for (var i = 0; i < TryCount; i++)
            {
                var port = GetRandomPort();
                if (PortList.Contains(port) is false)
                {
                    PortList.Add(port);
                    return port;
                }
            }

            throw new InvalidOperationException("Failed to find an available port.");
        }
    }

    public static int ReservePort(int port)
    {
        lock (LockObject)
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            listener.Stop();
            PortList.Add(port);
            return port;
        }
    }

    public static bool ContainsPort(int port) => PortList.Contains(port);

    private static int GetRandomPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
