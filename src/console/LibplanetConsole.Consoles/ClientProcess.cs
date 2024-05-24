using System.Diagnostics;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class ClientProcess : IDisposable
{
    private readonly Process _process;

    public ClientProcess(ClientProcessOptions options)
    {
        var isArm64Windows = IsWindows() && IsArm64();
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
            FileName = isArm64Windows ? ClientPath : "dotnet",
        };
        if (isArm64Windows != true)
        {
            startInfo.ArgumentList.Insert(0, ClientPath);
        }

        if (options.LogDirectory != string.Empty)
        {
            startInfo.ArgumentList.Add("--log-path");
            startInfo.ArgumentList.Add(
                Path.Combine(
                    options.LogDirectory,
                    $"{(ShortAddress)options.PrivateKey.Address}.log"));
        }

        var filename = startInfo.FileName;
        var arguments = CommandUtility.Join([.. startInfo.ArgumentList]);
        ApplicationLogger.Information(
            $"Starting a client process: {filename} {arguments}");
        _process = new Process
        {
            StartInfo = startInfo,
        };
        _process.ErrorDataReceived += Process_ErrorDataReceived;
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
            ApplicationLogger.Information(
                $"Closed the client process (PID: {_process.Id}).");
        }
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            ApplicationLogger.Error(text);
            Console.Error.WriteColoredLine(text, TerminalColorType.Red);
        }
    }
}
