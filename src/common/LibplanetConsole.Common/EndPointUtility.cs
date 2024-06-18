using System.Diagnostics.CodeAnalysis;
using System.Net;
using CommunicationUtility = JSSoft.Communication.EndPointUtility;

namespace LibplanetConsole.Common;

public static class EndPointUtility
{
    public static (string Host, int Port) GetElements(EndPoint endPoint)
        => CommunicationUtility.GetElements(endPoint);

    public static string ToString(EndPoint endPoint)
        => CommunicationUtility.ToString(endPoint);

    public static string ToSafeString(EndPoint? endPoint)
        => endPoint is not null ? CommunicationUtility.ToString(endPoint) : string.Empty;

    public static EndPoint Parse(string text)
        => CommunicationUtility.Parse(text);

    public static EndPoint ParseWithFallback(string text)
        => text == string.Empty ? DnsEndPointUtility.Next() : Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out EndPoint endPoint)
        => CommunicationUtility.TryParse(text, out endPoint);
}
