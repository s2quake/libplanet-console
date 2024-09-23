using System.Diagnostics;
using System.Runtime.InteropServices;
using LibplanetConsole.Console.Tests.Extensions;

namespace LibplanetConsole.Console.Tests;

public sealed class ConsoleApplicationFixture : IDisposable
{
    private readonly string _tempDirectory
        = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    public ConsoleApplicationFixture()
    {
        var assembly = typeof(ProcessTest).Assembly;
        var codePath = "LibplanetConsole.Console.Tests.ConsoleApplicationSource.cs";
        var code = assembly.GetManifestResourceString(codePath);
        var assemblyName = Path.GetRandomFileName();
        var assemblyPath = Path.Combine(_tempDirectory, $"{assemblyName}.dll");
        Directory.CreateDirectory(_tempDirectory);
        ConsoleApplicationCompiler.CompileCode(code, assemblyName, assemblyPath);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("chmod", $"+x {assemblyPath}").WaitForExit();
        }

        ExecutionPath = assemblyPath;
    }

    public string ExecutionPath { get; }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            try
            {
                Directory.Delete(_tempDirectory, true);
            }
            catch (UnauthorizedAccessException)
            {
                // ignore
            }
        }
    }
}
