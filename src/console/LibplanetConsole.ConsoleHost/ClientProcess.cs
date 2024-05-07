using System.Diagnostics;
using System.Net;
using JSSoft.Communication;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using static LibplanetConsole.ConsoleHost.ProcessUtility;

namespace LibplanetConsole.ConsoleHost;

internal sealed class ClientProcess : IDisposable
{
    private readonly Process _process;

    public ClientProcess(EndPoint endPoint, PrivateKey privateKey)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            ArgumentList =
            {
                ClientPath,
                "--end-point",
                EndPointUtility.ToString(endPoint),
                "--private-key",
                PrivateKeyUtility.ToString(privateKey),
                "--parent",
                Environment.ProcessId.ToString(),
            },
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        _process = new Process
        {
            StartInfo = startInfo,
        };
        _process.Start();
    }

    public event EventHandler? Exited
    {
        add => _process.Exited += value;
        remove => _process.Exited -= value;
    }

    public int Id => _process.Id;

    public void Dispose()
    {
        if (_process.HasExited != true)
        {
            _process.Close();
        }
    }
}
