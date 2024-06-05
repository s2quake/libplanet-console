using System.Diagnostics;
using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class NodeProcess(NodeProcessOptions options) : ProcessBase
{
    private readonly NodeProcessOptions _options = options;

    protected override ProcessStartInfo GetStartInfo()
    {
        var options = _options;
        var isDotnetRuntime = IsDotnetRuntime();
        var privateKey = PrivateKeyUtility.FromSecureString(options.PrivateKey);
        var startInfo = new ProcessStartInfo
        {
            ArgumentList =
            {
                "--end-point",
                EndPointUtility.ToString(options.EndPoint),
                "--private-key",
                PrivateKeyUtility.ToString(privateKey),
                "--parent",
                Environment.ProcessId.ToString(),
                "--manual-start",
            },
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        if (isDotnetRuntime == true)
        {
            startInfo.FileName = DotnetPath;
            startInfo.ArgumentList.Insert(0, NodePath);
        }
        else
        {
            startInfo.FileName = NodePath;
        }

        if (options.NoREPL == true)
        {
            startInfo.ArgumentList.Add("--no-repl");
        }

        if (options.StoreDirectory != string.Empty)
        {
            startInfo.ArgumentList.Add("--store-path");
            startInfo.ArgumentList.Add(
                Path.Combine(options.StoreDirectory, (ShortAddress)privateKey.Address));
        }

        if (options.LogDirectory != string.Empty)
        {
            startInfo.ArgumentList.Add("--log-path");
            startInfo.ArgumentList.Add(
                Path.Combine(
                    options.LogDirectory,
                    $"{(ShortAddress)privateKey.Address}.log"));
        }

        if (options.NodeEndPoint is { } nodeEndPoint)
        {
            startInfo.ArgumentList.Add("--node-end-point");
            startInfo.ArgumentList.Add(EndPointUtility.ToString(nodeEndPoint));
        }

        return startInfo;
    }
}
