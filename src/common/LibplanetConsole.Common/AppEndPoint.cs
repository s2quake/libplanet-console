// using System.Diagnostics.CodeAnalysis;
// using System.Net;
// using System.Net.Sockets;
// using System.Text.Json.Serialization;
// using LibplanetConsole.Common.Converters;
// using CommunicationUtility = JSSoft.Communication.EndPointUtility;

// namespace LibplanetConsole.Common;

// [JsonConverter(typeof(EndPointJsonConverter))]
// public sealed record class EndPoint
// {
//     public const string HostExpression
//         = @"(?:(?:[a-zA-Z0-9\-\.]+)|(?:\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}))";

//     public const string PortExpression = @"\d{1,5}";
//     public static readonly string RegularExpression
//         = $"{HostExpression}:{PortExpression}";

//     private static readonly object LockObject = new();
//     private static readonly List<int> PortList = [];

//     public EndPoint(string host, int port)
//     {
//         Host = host;
//         Port = port;
//     }

//     public string Host { get; }

//     public int Port { get; }

//     public static explicit operator EndPoint(EndPoint endPoint)
//         => new DnsEndPoint(endPoint.Host, endPoint.Port);

//     public static explicit operator DnsEndPoint(EndPoint endPoint)
//         => new(endPoint.Host, endPoint.Port);

//     public static explicit operator EndPoint(EndPoint endPoint)
//     {
//         if (endPoint is DnsEndPoint dnsEndPoint)
//         {
//             return new(dnsEndPoint.Host, dnsEndPoint.Port);
//         }

//         if (endPoint is IPEndPoint ipEndPoint)
//         {
//             return new($"{ipEndPoint.Address}", ipEndPoint.Port);
//         }

//         throw new InvalidCastException();
//     }

//     public static EndPoint Next() => new("localhost", GetPort());

//     public static EndPoint Parse(string text) => (EndPoint)CommunicationUtility.Parse(text);

//     public static EndPoint ParseOrNext(string text)
//         => text == string.Empty ? Next() : Parse(text);

//     public static EndPoint ParseOrFallback(string text, EndPoint fallback)
//         => text == string.Empty ? fallback : Parse(text);

//     public static EndPoint? ParseOrDefault(string text)
//         => text == string.Empty ? null : Parse(text);

//     public static bool TryParse(string text, [MaybeNullWhen(false)] out EndPoint endPoint)
//     {
//         if (CommunicationUtility.TryParse(text, out var value))
//         {
//             endPoint = (EndPoint)value;
//             return true;
//         }

//         endPoint = null;
//         return false;
//     }

//     public static string ToString(EndPoint? endPoint)
//         => endPoint is not null ? endPoint.ToString() : string.Empty;

//     public override string ToString() => $"{Host}:{Port}";

//     private static int GetPort()
//     {
//         lock (LockObject)
//         {
//             var port = GetRandomPort();
//             while (PortList.Contains(port) == true)
//             {
//                 port = GetRandomPort();
//             }

//             PortList.Add(port);
//             return port;
//         }
//     }

//     private static int GetRandomPort()
//     {
//         var listener = new TcpListener(IPAddress.Loopback, 0);
//         listener.Start();
//         var port = ((IPEndPoint)listener.LocalEndpoint).Port;
//         listener.Stop();
//         return port;
//     }
// }
