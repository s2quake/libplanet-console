using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Text.Json;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;
using Microsoft.AspNetCore.Http;
using CommunicationUtility = JSSoft.Communication.EndPointUtility;

namespace LibplanetConsole.Common;

public static class EndPointUtility
{
    private static readonly object LockObject = new();
    private static readonly List<int> PortList = [];

    public static EndPoint Next() => new DnsEndPoint("localhost", GetPort());

    public static EndPoint Parse(string text) => CommunicationUtility.Parse(text);

    public static EndPoint ParseOrNext(string text)
        => text == string.Empty ? Next() : Parse(text);

    public static EndPoint ParseOrFallback(string text, EndPoint fallback)
        => text == string.Empty ? fallback : Parse(text);

    public static EndPoint? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out EndPoint endPoint)
    {
        if (CommunicationUtility.TryParse(text, out var value))
        {
            endPoint = value;
            return true;
        }

        endPoint = null;
        return false;
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

    // public static string ToString(EndPoint? endPoint)
    //     => endPoint is not null ? endPoint.ToString() : string.Empty;

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
