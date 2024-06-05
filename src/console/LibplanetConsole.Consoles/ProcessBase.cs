using System.Diagnostics;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.Common.Threading;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

internal abstract class ProcessBase : IDisposable
{
    private Process? _process;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler? Exited;

    public int Id => _process?.Id ?? -1;

    public bool StartOnTerminal { get; set; }

    public bool Start()
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("The process is already disposed.");
        }

        _process = GetProcess();
        if (_process.Start() != true)
        {
            return false;
        }

        _cancellationTokenSource = new();
        ObserveExit(_cancellationTokenSource.Token);
        return true;
    }

    private Process GetProcess()
    {
        var startInfo = GetStartInfo();
        if (StartOnTerminal == true)
        {
            var filename = startInfo.FileName;
            var arguments = CommandUtility.Join([.. startInfo.ArgumentList]);
            var script = $"tell application \"Terminal\"\n" +
                         $"  do script \"{filename} {arguments}; exit\"\n" +
                         $"  activate\n" +
                         $"end tell\n";
            var tempFile = TempFile.WriteAllText(script);

            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/osascript",
                    ArgumentList =
                {
                    tempFile.FileName,
                },
                },
            };
        }

        return new Process() { StartInfo = startInfo };
    }

    public void Close()
    {
        if (_process is null)
        {
            throw new InvalidOperationException("The process is already disposed.");
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _process.Close();
        _process = null;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _process?.Dispose();
        _process = null;
    }

    protected abstract ProcessStartInfo GetStartInfo();

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            ApplicationLogger.Error(text);
            Console.Error.WriteColoredLine(text, TerminalColorType.Red);
        }
    }

    private async void ObserveExit(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested != true)
        {
            if (_process is null)
            {
                break;
            }

            if (_process?.HasExited == true)
            {
                Exited?.Invoke(this, EventArgs.Empty);
                break;
            }

            if (await TaskUtility.Delay(100, cancellationToken) != true)
            {
                break;
            }
        }
    }
}
