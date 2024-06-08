using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.IO;
using LibplanetConsole.Common.Threading;
using LibplanetConsole.Frameworks;
using static LibplanetConsole.Consoles.ProcessUtility;

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

    public bool NewWindow { get; set; }

    protected abstract string FileName { get; }

    protected abstract Collection<string> ArgumentList { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("The process is already disposed.");
        }

        _process = GetProcess();
        if (NewWindow != true)
        {
            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.ErrorDataReceived += Process_ErrorDataReceived;
        }

        ApplicationLogger.Debug("Process staring: " + JsonUtility.SerializeObject(new
        {
            HashCode = _process.GetHashCode(),
            _process.StartInfo.FileName,
            Arguments = GetArguments(_process.StartInfo),
            _process.StartInfo.WorkingDirectory,
        }));

        if (_process.Start() != true)
        {
            throw new InvalidOperationException("Failed to start the process.");
        }

        ApplicationLogger.Debug("Process started: " + JsonUtility.SerializeObject(new
        {
            HashCode = _process.GetHashCode(),
            _process.Id,
        }));

        if (NewWindow != true)
        {
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        await Task.Delay(1, cancellationToken);
        if (_process.HasExited == true)
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

    private static string GetArguments(ProcessStartInfo processStartInfo)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var methodInfo = typeof(ProcessStartInfo).GetMethod("BuildArguments", bindingFlags)
            ?? throw new MissingMethodException("ProcessStartInfo", "BuildArguments");
        if (methodInfo.Invoke(processStartInfo, null) is string arguments)
        {
            return arguments;
        }

        throw new InvalidOperationException("Failed to get arguments.");
    }

    private Process GetProcess()
    {
        var startInfo = GetProcessStartInfo();
        startInfo.CreateNoWindow = NewWindow != true;
        startInfo.UseShellExecute = IsWindows() == true && NewWindow == true;
        startInfo.RedirectStandardOutput = NewWindow != true;
        startInfo.RedirectStandardError = NewWindow != true;
        startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
        return new Process { StartInfo = startInfo, };
    }

    private ProcessStartInfo GetProcessStartInfo()
    {
        if (NewWindow == true)
        {
            if (IsOSX() == true)
            {
                return GetProcessStartInfoOnMacOS();
            }

            if (IsWindows() == true)
            {
                return GetProcessStartInfoOnWindows();
            }

            if (IsLinux() == true)
            {
                return GetProcessStartInfoOnLinux();
            }

            throw new NotSupportedException("The new terminal is not supported on this platform.");
        }

        return new ProcessStartInfo(FileName, ArgumentList);
    }

    private ProcessStartInfo GetProcessStartInfoOnMacOS()
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

    private ProcessStartInfo GetProcessStartInfoOnWindows()
    {
        var filename = $"\"{FileName}\"";
        var arguments = CommandUtility.Join([.. ArgumentList]);
        var tempFile = TempFile.WriteAllText($"& {filename} {arguments}");
        var scriptList = new List<string>
        {
            $"Invoke-Expression $(Get-Content {tempFile})",
            "Write-Host -NoNewLine 'Press any key to exit.'",
            "$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')",
        };
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            ArgumentList =
            {
                "-Command",
                $"& {{ {string.Join(';', scriptList)} }}",
            },
        };

        return startInfo;
    }

    private ProcessStartInfo GetProcessStartInfoOnLinux()
    {
        var filename = FileName;
        var arguments = CommandUtility.Join([.. ArgumentList]);

        return new ProcessStartInfo
        {
            FileName = "/usr/bin/gnome-terminal",
            ArgumentList =
            {
                "--",
                "/bin/bash",
                "-c",
                $"{filename} {arguments}; read -n1 -r -p 'Press any key to exit...'",
            },
        };
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
