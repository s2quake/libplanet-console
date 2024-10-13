using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace LibplanetConsole.Common;

public static class EndPointUtility
{
    private static readonly object LockObject = new();
    private static readonly List<int> PortList = [];

    public static EndPoint NextEndPoint() => new DnsEndPoint("localhost", GetPort());

    public static EndPoint Parse(string text)
    {
        var items = text.Split(':');
        if (IPAddress.TryParse(items[0], out var address) == true)
        {
            return new IPEndPoint(address, int.Parse(items[1]));
        }
        else if (items.Length == 2)
        {
            return new DnsEndPoint(items[0], int.Parse(items[1]));
        }

        throw new NotSupportedException($"'{text}' is not supported.");
    }

    public static EndPoint ParseOrNext(string text)
        => text == string.Empty ? NextEndPoint() : Parse(text);

    public static EndPoint ParseOrFallback(string text, EndPoint fallback)
        => text == string.Empty ? fallback : Parse(text);

    public static EndPoint? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out EndPoint endPoint)
    {
        try
        {
            endPoint = Parse(text);
            return true;
        }
        catch
        {
            endPoint = null;
            return false;
        }
    }

    public static (string Host, int Port) GetHostAndPort(EndPoint endPoint)
    {
        return endPoint switch
        {
            DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
            IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
            _ => throw new NotSupportedException($"Unsupported EndPoint type: {endPoint}."),
        };
    }

    public static string GetHost(EndPoint endPoint)
        => GetHostAndPort(endPoint).Host;

    public static int GetPort(EndPoint endPoint)
        => GetHostAndPort(endPoint).Port;

    public static DnsEndPoint GetLocalHost(int port) => new("localhost", port);

    public static string ToString(EndPoint? endPoint)
    {
        if (endPoint is DnsEndPoint dnsEndPoint)
        {
            return $"{dnsEndPoint.Host}:{dnsEndPoint.Port}";
        }
        else if (endPoint is IPEndPoint ipEndPoint)
        {
            return $"{ipEndPoint.Address}:{ipEndPoint.Port}";
        }

        return endPoint?.ToString() ?? string.Empty;
    }

    public static bool CompareEndPoint(EndPoint? endPoint1, EndPoint? endPoint2)
    {
        if (endPoint1 is null && endPoint2 is null)
        {
            return true;
        }

        if (endPoint1 is null || endPoint2 is null)
        {
            return false;
        }

        var (host1, port1) = GetHostAndPort(endPoint1);
        var (host2, port2) = GetHostAndPort(endPoint2);
        return host1 == host2 && port1 == port2;
    }

    private static int GetPort()
    {
        lock (LockObject)
        {
            var port = GetRandomPort();
            while (PortList.Contains(port) == true)
            {
                port = GetRandomPort();
            }

            PortList.Add(port);
            return port;
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
