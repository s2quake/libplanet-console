using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;
using CommunicationUtility = JSSoft.Communication.EndPointUtility;

namespace LibplanetConsole.Common;

[JsonConverter(typeof(AppEndPointJsonConverter))]
public sealed record class AppEndPoint
{
    private static readonly object LockObject = new();
    private static readonly List<int> PortList = [];

    public AppEndPoint(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public string Host { get; }

    public int Port { get; }

    public static explicit operator EndPoint(AppEndPoint endPoint)
        => new DnsEndPoint(endPoint.Host, endPoint.Port);

    public static explicit operator DnsEndPoint(AppEndPoint endPoint)
        => new(endPoint.Host, endPoint.Port);

    public static explicit operator AppEndPoint(EndPoint endPoint)
    {
        if (endPoint is DnsEndPoint dnsEndPoint)
        {
            return new(dnsEndPoint.Host, dnsEndPoint.Port);
        }

        if (endPoint is IPEndPoint ipEndPoint)
        {
            return new($"{ipEndPoint.Address}", ipEndPoint.Port);
        }

        throw new InvalidCastException();
    }

    public static AppEndPoint Next() => new("localhost", GetPort());

    public static AppEndPoint Parse(string text) => (AppEndPoint)CommunicationUtility.Parse(text);

    public static AppEndPoint ParseOrNext(string text)
        => text == string.Empty ? Next() : Parse(text);

    public static AppEndPoint ParseOrFallback(string text, AppEndPoint fallback)
        => text == string.Empty ? fallback : Parse(text);

    public static AppEndPoint? ParseOrDefault(string text)
        => text == string.Empty ? null : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out AppEndPoint endPoint)
    {
        if (CommunicationUtility.TryParse(text, out var value))
        {
            endPoint = (AppEndPoint)value;
            return true;
        }

        endPoint = null;
        return false;
    }

    public static string ToString(AppEndPoint? endPoint)
        => endPoint is not null ? endPoint.ToString() : string.Empty;

    public override string ToString() => $"{Host}:{Port}";

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
