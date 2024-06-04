using System.Diagnostics;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class NodeProcess : IDisposable
{
    private readonly Process _process;

    public NodeProcess(NodeProcessOptions options)
    {
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
            startInfo.ArgumentList.Insert(0, NodePath);
        }
        else
        {
            startInfo.FileName = NodePath;
        }

        if (options.StoreDirectory != string.Empty)
        {
            startInfo.ArgumentList.Add("--store-path");
            startInfo.ArgumentList.Add(
                Path.Combine(options.StoreDirectory, (ShortAddress)options.PrivateKey.Address));
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
            $"Starting a node process: {filename} {arguments}");
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
                $"Closed the node process (PID: {_process.Id}).");
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
