using System.Security;
using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal sealed class ClientProcess(Client client) : ProcessBase
{
    public required AppEndPoint EndPoint { get; init; }

    public required SecureString PrivateKey { get; init; }

    public AppEndPoint? NodeEndPoint { get; set; }

    public string LogDirectory { get; set; } = string.Empty;

    public bool ManualStart { get; set; }

    protected override string FileName => IsDotnetRuntime ? DotnetPath : ClientPath;

    protected override string[] Arguments
    {
        get
        {
            var privateKey = AppPrivateKey.FromSecureString(PrivateKey);
            var argumentList = new List<string>
            {
                "run",
                "--end-point",
                EndPoint.ToString(),
                "--private-key",
                AppPrivateKey.ToString(privateKey),
            };

            if (IsDotnetRuntime == true)
            {
                argumentList.Insert(0, ClientPath);
            }

            if (NewWindow != true)
            {
                argumentList.Add("--no-repl");
            }

            if (LogDirectory != string.Empty)
            {
                var logFilename = $"client-{privateKey.Address:S}.log";
                var logPath = Path.Combine(LogDirectory, logFilename);
                argumentList.Add("--log-path");
                argumentList.Add(logPath);
            }

            if (NodeEndPoint is { } nodeEndPoint)
            {
                argumentList.Add("--node-end-point");
                argumentList.Add(nodeEndPoint.ToString());
            }

            if (Detach != true)
            {
                argumentList.Add("--parent");
                argumentList.Add(Environment.ProcessId.ToString());
            }

            var extendedArguments = GetArguments(serviceProvider: client, obj: client);
            argumentList.AddRange(extendedArguments);

            return [.. argumentList];
        }
    }
}
