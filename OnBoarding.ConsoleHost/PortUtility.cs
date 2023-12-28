using System.Net;
using System.Net.Sockets;

namespace OnBoarding.ConsoleHost;

static class PortUtility
{
    private static readonly List<int> reservedPortList = new();
    private static readonly List<int> usedPortList = new();

    public static int GetPort()
    {
        if (reservedPortList.Count == 0)
        {
            var v = GetRandomPort();
            while (reservedPortList.Contains(v) == true || usedPortList.Contains(v) == true)
            {
                v = GetRandomPort();
            }
            reservedPortList.Add(v);
        }
        var port = reservedPortList[0];
        reservedPortList.Remove(port);
        usedPortList.Add(port);
        return port;
    }

    public static void ReleasePort(int port)
    {
        reservedPortList.Add(port);
        usedPortList.Remove(port);
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
