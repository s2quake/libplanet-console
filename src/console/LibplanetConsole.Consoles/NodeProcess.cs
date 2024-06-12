using System.Net;
using System.Security;
using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class NodeProcess(Node node) : ProcessBase
{
    public required EndPoint EndPoint { get; init; }

    public required SecureString PrivateKey { get; init; }

    public EndPoint? NodeEndPoint { get; set; }

    public string StoreDirectory { get; set; } = string.Empty;

    public string LogDirectory { get; set; } = string.Empty;

    public bool ManualStart { get; set; }

    protected override string FileName => IsDotnetRuntime() ? DotnetPath : NodePath;

    protected override string[] Arguments
    {
        get
        {
            var privateKey = PrivateKeyUtility.FromSecureString(PrivateKey);
            var argumentList = new List<string>
            {
                "--end-point",
                EndPointUtility.ToString(EndPoint),
                "--private-key",
                PrivateKeyUtility.ToString(privateKey),
            };

            if (IsDotnetRuntime() == true)
            {
                argumentList.Insert(0, NodePath);
            }

            if (NewWindow != true)
            {
                argumentList.Add("--no-repl");
            }

            if (StoreDirectory != string.Empty)
            {
                argumentList.Add("--store-path");
                argumentList.Add(
                    Path.Combine(StoreDirectory, (ShortAddress)privateKey.Address));
            }

            if (LogDirectory != string.Empty)
            {
                argumentList.Add("--log-path");
                argumentList.Add(
                    Path.Combine(
                        LogDirectory,
                        $"node-{(ShortAddress)privateKey.Address}.log"));
            }

            if (NodeEndPoint is { } nodeEndPoint)
            {
                argumentList.Add("--node-end-point");
                argumentList.Add(EndPointUtility.ToString(nodeEndPoint));
            }

            if (ManualStart == true)
            {
                argumentList.Add("--manual-start");
            }

            if (Detach != true)
            {
                argumentList.Add("--parent");
                argumentList.Add(Environment.ProcessId.ToString());
            }

            var extendedArguments = GetArguments(serviceProvider: node, obj: node);
            argumentList.AddRange(extendedArguments);

            return [.. argumentList];
        }
    }
}
