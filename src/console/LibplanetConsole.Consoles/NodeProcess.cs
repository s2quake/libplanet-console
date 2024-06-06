using System.Collections.ObjectModel;
using System.Net;
using System.Security;
using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class NodeProcess : ProcessBase
{
    public required EndPoint EndPoint { get; init; }

    public required SecureString PrivateKey { get; init; }

    public EndPoint? NodeEndPoint { get; set; }

    public string StoreDirectory { get; set; } = string.Empty;

    public string LogDirectory { get; set; } = string.Empty;

    public bool ManualStart { get; set; }

    protected override string FileName => IsDotnetRuntime() ? DotnetPath : NodePath;

    protected override Collection<string> ArgumentList
    {
        get
        {
            var privateKey = PrivateKeyUtility.FromSecureString(PrivateKey);
            var argumentList = new Collection<string>
            {
                "--end-point",
                EndPointUtility.ToString(EndPoint),
                "--private-key",
                PrivateKeyUtility.ToString(privateKey),
                "--parent",
                Environment.ProcessId.ToString(),
            };

            if (IsDotnetRuntime() == true)
            {
                argumentList.Insert(0, NodePath);
            }

            if (NewTerminal != true)
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
                        $"{(ShortAddress)privateKey.Address}.log"));
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

            return argumentList;
        }
    }
}
