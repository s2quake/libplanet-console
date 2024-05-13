using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace LibplanetConsole.Common;

public static class EndPointUtility
{
    public static (string Host, int Port) GetElements(EndPoint endPoint)
        => JSSoft.Communication.EndPointUtility.GetElements(endPoint);

    public static string ToString(EndPoint endPoint)
        => JSSoft.Communication.EndPointUtility.ToString(endPoint);

    public static EndPoint Parse(string text)
        => JSSoft.Communication.EndPointUtility.Parse(text);

    public static bool TryParse(string text, [MaybeNullWhen(false)] out EndPoint endPoint)
        => JSSoft.Communication.EndPointUtility.TryParse(text, out endPoint);
}
