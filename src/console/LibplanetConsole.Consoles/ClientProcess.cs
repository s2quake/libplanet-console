using System.Diagnostics;
using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class ClientProcess(ClientProcessOptions options) : ProcessBase
{
    private readonly ClientProcessOptions _options = options;

    protected override ProcessStartInfo GetStartInfo()
    {
        var options = _options;
        var isDotnetRuntime = IsDotnetRuntime();
        var startInfo = new ProcessStartInfo
        {
            ArgumentList =
            {
                "--end-point",
                EndPointUtility.ToString(options.EndPoint),
                "--private-key",
                PrivateKeyUtility.ToString(options.PrivateKey),
                "--parent",
                Environment.ProcessId.ToString(),
            },
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        if (isDotnetRuntime == true)
        {
            startInfo.FileName = DotnetPath;
            startInfo.ArgumentList.Insert(0, ClientPath);
        }
        else
        {
            startInfo.FileName = ClientPath;
        }

        if (options.NoREPL == true)
        {
            startInfo.ArgumentList.Add("--no-repl");
        }

        if (options.LogDirectory != string.Empty)
        {
            startInfo.ArgumentList.Add("--log-path");
            startInfo.ArgumentList.Add(
                Path.Combine(
                    options.LogDirectory,
                    $"{(ShortAddress)options.PrivateKey.Address}.log"));
        }

        return startInfo;
    }
}
