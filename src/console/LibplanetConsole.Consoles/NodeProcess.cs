using System.Security;
using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal sealed class NodeProcess(Node node) : ProcessBase
{
    public required AppEndPoint EndPoint { get; init; }

    public required SecureString PrivateKey { get; init; }

    public AppEndPoint? NodeEndPoint { get; set; }

    public string StoreDirectory { get; set; } = string.Empty;

    public string LogDirectory { get; set; } = string.Empty;

    public bool ManualStart { get; set; }

    protected override string FileName => IsDotnetRuntime ? DotnetPath : NodePath;

    protected override string[] Arguments
    {
        get
        {
            var privateKey = AppPrivateKey.FromSecureString(PrivateKey);
            var argumentList = new List<string>
            {
                "--end-point",
                EndPoint.ToString(),
                "--private-key",
                AppPrivateKey.ToString(privateKey),
            };

            if (IsDotnetRuntime == true)
            {
                argumentList.Insert(0, NodePath);
            }

            if (NewWindow != true)
            {
                argumentList.Add("--no-repl");
            }

            if (StoreDirectory != string.Empty)
            {
                var storePath = Path.Combine(StoreDirectory, $"{privateKey.Address}:S");
                argumentList.Add("--store-path");
                argumentList.Add(storePath);
            }

            if (LogDirectory != string.Empty)
            {
                var logFilename = $"node-{privateKey.Address:S}.log";
                var logPath = Path.Combine(LogDirectory, logFilename);
                argumentList.Add("--log-path");
                argumentList.Add(logPath);
            }

            if (NodeEndPoint is { } nodeEndPoint)
            {
                argumentList.Add("--node-end-point");
                argumentList.Add(nodeEndPoint.ToString());
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
