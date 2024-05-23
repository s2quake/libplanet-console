using System.Diagnostics;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using static LibplanetConsole.Consoles.ProcessUtility;

namespace LibplanetConsole.Consoles;

internal sealed class NodeProcess : IDisposable
{
    private readonly Process _process;

    public NodeProcess(NodeProcessOptions options)
    {
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
            FileName = IsWindows() ? NodePath : "dotnet",
        };
        if (IsWindows() != true)
        {
            startInfo.ArgumentList.Insert(0, NodePath);
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
        }
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            Console.Error.WriteColoredLine(text, TerminalColorType.Red);
        }
    }
}
