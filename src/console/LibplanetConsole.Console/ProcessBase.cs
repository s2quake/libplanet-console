using System.Diagnostics;
using System.Reflection;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using static LibplanetConsole.Console.ProcessEnvironment;

namespace LibplanetConsole.Console;

public abstract class ProcessBase
{
    private const int MillisecondsDelay = 10000;

    private readonly StringBuilder _errorBuilder = new();
    private Process? _process;
    private TaskCompletionSource? _errorTaskCompletionSource;

    public event DataReceivedEventHandler? ErrorDataReceived;

    public event DataReceivedEventHandler? OutputDataReceived;

    public int Id => _process?.Id ?? -1;

    public bool IsRunning => _process is not null;

    public bool NewWindow { get; set; }

    public string WorkingDirectory { get; set; } = string.Empty;

    public virtual bool SupportsDotnetRuntime => false;

    public abstract string FileName { get; }

    public abstract string[] Arguments { get; }

    public override string? ToString() => GetArguments(GetProcessStartInfo());

    public void Run() => Run(MillisecondsDelay);

    public void Run(int millisecondsDelay)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("The process is already running.");
        }

        _process = new Process { StartInfo = GetProcessStartInfo(), };

        try
        {
            if (NewWindow is not true)
            {
                _errorTaskCompletionSource = new TaskCompletionSource();
                _process.OutputDataReceived += Process_OutputDataReceived;
                _process.ErrorDataReceived += Process_ErrorDataReceived;
            }

            if (_process.Start() is not true)
            {
                throw new InvalidOperationException("Failed to start the process.");
            }

            if (NewWindow is not true)
            {
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
            }

            if (_process.WaitForExit(millisecondsDelay) is true)
            {
                _errorTaskCompletionSource?.Task.Wait();
                if (_process.ExitCode != 0)
                {
                    throw new ProcessExecutionException(_errorBuilder.ToString(), _process.ExitCode)
                    {
                        CommandLine = GetCommandLine(),
                    };
                }
            }
            else
            {
                _process.Kill();
                throw new OperationCanceledException("The process is not exited.");
            }
        }
        finally
        {
            _errorBuilder.Clear();
            _errorTaskCompletionSource = null;
            _process.Dispose();
            _process = null;
        }
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            throw new InvalidOperationException("The process is already running.");
        }

        _process = new Process { StartInfo = GetProcessStartInfo(), };

        try
        {
            if (NewWindow is not true)
            {
                _errorTaskCompletionSource = new TaskCompletionSource();
                _process.OutputDataReceived += Process_OutputDataReceived;
                _process.ErrorDataReceived += Process_ErrorDataReceived;
            }

            if (_process.Start() is not true)
            {
                throw new InvalidOperationException("Failed to start the process.");
            }

            if (NewWindow is not true)
            {
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
            }

            await _process.WaitForExitAsync(cancellationToken);
            if (_process.ExitCode is not 0)
            {
                if (_errorTaskCompletionSource is not null)
                {
                    await _errorTaskCompletionSource.Task;
                }

                if (_process.ExitCode != 0)
                {
                    throw new ProcessExecutionException(_errorBuilder.ToString(), _process.ExitCode)
                    {
                        CommandLine = GetCommandLine(),
                    };
                }
            }
        }
        catch (OperationCanceledException)
        {
            _process.Kill();
            throw;
        }
        finally
        {
            _errorBuilder.Clear();
            _errorTaskCompletionSource = null;
            _process.Dispose();
            _process = null;
        }
    }

    public string GetCommandLine()
    {
        var filename = GetFileName();
        var arguments = GetArguments(GetProcessStartInfo());
        var items = new string[] { filename, arguments };
        return string.Join(" ", items.Where(item => item != string.Empty));
    }

    // Reflection should not be used to increase accessibility of classes, methods, or fields
#pragma warning disable S3011
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
#pragma warning restore S3011

    private ProcessStartInfo GetProcessStartInfo()
    {
        var startInfo = GetProcessStartInfoByPlatform();
        startInfo.CreateNoWindow = NewWindow is not true;
        startInfo.UseShellExecute = IsWindows is true && NewWindow is true;
        startInfo.RedirectStandardOutput = NewWindow is not true;
        startInfo.RedirectStandardError = NewWindow is not true;
        startInfo.RedirectStandardInput = true;
        startInfo.WorkingDirectory = WorkingDirectory.Fallback(Directory.GetCurrentDirectory());
        return startInfo;
    }

    private ProcessStartInfo GetProcessStartInfoByPlatform()
    {
        if (NewWindow is true)
        {
            if (IsOSX is true)
            {
                return GetProcessStartInfoOnMacOS();
            }

            if (IsWindows is true)
            {
                return GetProcessStartInfoOnWindows();
            }

            if (IsLinux is true)
            {
                return GetProcessStartInfoOnLinux();
            }

            throw new NotSupportedException("The new terminal is not supported on this platform.");
        }

        return new ProcessStartInfo(GetFileName(), GetArguments());
    }

    private string GetFileName()
    {
        if (SupportsDotnetRuntime is true && IsDotnetRuntime is true)
        {
            return DotnetPath;
        }

        return FileName;
    }

    private string[] GetArguments()
    {
        if (SupportsDotnetRuntime is true && IsDotnetRuntime is true)
        {
            return [FileName, .. Arguments];
        }

        return Arguments;
    }

    private ProcessStartInfo GetProcessStartInfoOnMacOS()
    {
        var filename = GetFileName();
        var arguments = CommandUtility.Join(GetArguments());
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
        var filename = $"\"{GetFileName()}\"";
        var arguments = CommandUtility.Join(GetArguments());
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
        var filename = GetFileName();
        var arguments = CommandUtility.Join(GetArguments());

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
        OutputDataReceived?.Invoke(sender, e);
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            if (_errorBuilder.Length > 0)
            {
                _errorBuilder.AppendLine();
            }

            _errorBuilder.Append(text);
        }
        else
        {
            _errorTaskCompletionSource?.SetResult();
        }

        ErrorDataReceived?.Invoke(sender, e);
    }
}
