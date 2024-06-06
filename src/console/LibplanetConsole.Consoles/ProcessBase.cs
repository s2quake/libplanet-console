using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common.IO;
using LibplanetConsole.Common.Threading;

namespace LibplanetConsole.Consoles;

internal abstract class ProcessBase : IAsyncDisposable
{
    private readonly StringBuilder _outBuilder = new();
    private readonly StringBuilder _errorBuilder = new();
    private Process? _process;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _exitTask = Task.CompletedTask;

    public int Id => _process?.Id ?? -1;

    public bool IsRunning => _process?.HasExited != true;

    public bool NewTerminal { get; set; }

    protected abstract string FileName { get; }

    protected abstract Collection<string> ArgumentList { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("The process is already disposed.");
        }

        _process = GetProcess();
        _process.OutputDataReceived += Process_OutputDataReceived;
        _process.ErrorDataReceived += Process_ErrorDataReceived;
        _process.Start();

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        await Task.Delay(1000, cancellationToken);
        if (_process.ExitCode != 0)
        {
            throw new InvalidOperationException(_errorBuilder.ToString());
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _exitTask = WaitForExitAsync(_cancellationTokenSource.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_process is null)
        {
            throw new InvalidOperationException("The process is already disposed.");
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _process.Close();
        await _process.WaitForExitAsync(cancellationToken);
        _process = null;
        await _exitTask;
    }

    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _process?.Dispose();
        _process = null;
        await _exitTask;
    }

    public string GetCommandLine()
    {
        var filename = FileName;
        var arguments = CommandUtility.Join([.. ArgumentList]);
        return $"{filename} {arguments}";
    }

    private Process GetProcess()
    {
        var startInfo = GetProcessStartInfo();
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        return new Process { StartInfo = startInfo, };
    }

    private ProcessStartInfo GetProcessStartInfo()
    {
        if (NewTerminal == true)
        {
            var filename = FileName;
            var arguments = CommandUtility.Join([.. ArgumentList]);
            var script = $"tell application \"Terminal\"\n" +
                         $"  do script \"{filename} {arguments}; exit\"\n" +
                         $"  activate\n" +
                         $"end tell\n";
            var tempFile = TempFile.WriteAllText(script);

            return new ProcessStartInfo
            {
                FileName = "/usr/bin/osascript",
                ArgumentList =
                {
                    tempFile.FileName,
                },
            };
        }

        return new ProcessStartInfo(FileName, ArgumentList);
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            // _outBuilder.AppendLine(text);
        }
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            // _errorBuilder.AppendLine(text);
        }
    }

    private async Task WaitForExitAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested != true)
        {
            if (_process is null)
            {
                break;
            }

            if (_process?.HasExited == true)
            {
                break;
            }

            await TaskUtility.Delay(100, cancellationToken);
        }
    }
}
