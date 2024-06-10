using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace LibplanetConsole.Common;

public static class EndPointUtility
{
    public static (string Host, int Port) GetElements(EndPoint endPoint)
        => JSSoft.Communication.EndPointUtility.GetElements(endPoint);

    public static string ToString(EndPoint endPoint)
        => JSSoft.Communication.EndPointUtility.ToString(endPoint);

    public static string ToSafeString(EndPoint? endPoint)
    {
        if (endPoint is not null)
        {
            return JSSoft.Communication.EndPointUtility.ToString(endPoint);
        }

        return string.Empty;
    }

    public static EndPoint Parse(string text)
        => JSSoft.Communication.EndPointUtility.Parse(text);

    public static EndPoint ParseWithFallback(string text)
        => text == string.Empty ? DnsEndPointUtility.Next() : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out EndPoint endPoint)
        => JSSoft.Communication.EndPointUtility.TryParse(text, out endPoint);
}
