using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using CommunicationUtility = JSSoft.Communication.EndPointUtility;

namespace LibplanetConsole.Common;

public sealed class AppEndPoint : IDisposable
{
    private static readonly object LockObject = new();
    private static readonly List<int> PortList = [];

    private readonly EndPoint _endPoint;
    private bool _isDisposed;

    public AppEndPoint(string host, int port)
    {
        _endPoint = new DnsEndPoint(host, port);
        Host = host;
        Port = port;
    }

    private AppEndPoint(EndPoint endPoint)
    {
        _endPoint = endPoint;
        (Host, Port) = CommunicationUtility.GetElements(endPoint);
    }

    public string Host { get; }

    public int Port { get; }

    public static explicit operator EndPoint(AppEndPoint endPointObject)
        => endPointObject._endPoint;

    public static AppEndPoint Next() => new("localhost", GetPort());

    public static AppEndPoint Parse(string text) => new(CommunicationUtility.Parse(text));

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
            endPoint = new(value);
            return true;
        }

        endPoint = null;
        return false;
    }

    public static string ToString(AppEndPoint? endPoint)
        => endPoint is not null ? endPoint.ToString() : string.Empty;

    public override string ToString() => $"{Host}:{Port}";

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        lock (LockObject)
        {
            PortList.Remove(Port);
        }

        _isDisposed = true;
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
