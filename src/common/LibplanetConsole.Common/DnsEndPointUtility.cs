using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace LibplanetConsole.Common;

public static class DnsEndPointUtility
{
    public static string ToString(DnsEndPoint endPoint)
         => $"{endPoint.Host}:{endPoint.Port}";

    public static string ToSafeString(DnsEndPoint? endPoint)
         => endPoint is not null ? $"{endPoint.Host}:{endPoint.Port}" : string.Empty;

    public static DnsEndPoint GetEndPoint(string endPoint)
    {
        var items = endPoint.Split(':');
        if (items.Length == 2)
        {
            return new DnsEndPoint(items[0], int.Parse(items[1]));
        }

        throw new NotSupportedException($"'{endPoint}' is not supported.");
    }

    public static DnsEndPoint? GetSafeEndPoint(string endPoint)
    {
        var items = endPoint.Split(':');
        if (items.Length == 2)
        {
            return new DnsEndPoint(items[0], int.Parse(items[1]));
        }

        return null;
    }

    public static bool TryParse(string text, [MaybeNullWhen(false)] out DnsEndPoint value)
    {
        var items = text.Split(':');
        if (items.Length == 2 && int.TryParse(items[1], out var port) == true)
        {
            value = new DnsEndPoint(items[0], port);
            return true;
        }

        value = default;
        return false;
    }

    public static DnsEndPoint Next()
    {
        return new DnsEndPoint($"{IPAddress.Loopback}", PortUtility.GetPort());
    }

    public static void Release(ref DnsEndPoint? endPoint)
    {
        if (endPoint is not null)
        {
            PortUtility.ReleasePort(endPoint.Port);
            endPoint = null;
        }
    }
}
