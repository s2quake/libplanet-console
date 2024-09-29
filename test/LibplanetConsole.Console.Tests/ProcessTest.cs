using System.Globalization;
using Xunit.Extensions.AssemblyFixture;

namespace LibplanetConsole.Console.Tests;

public sealed class ProcessTest(ConsoleApplicationFixture consoleApplicationFixture)
    : IAssemblyFixture<ConsoleApplicationFixture>
{
    [Fact]
    public void Run_ThrowTest()
    {
        var process = new TestProcess("test", "arg1", "arg2");
        var exception = Assert.Throws<ProcessExecutionException>(process.Run);
        Assert.Equal(2, exception.ExitCode);
        Assert.Equal("test arg1 arg2", exception.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_ThrowTestAsync()
    {
        var process = new TestProcess("test", "arg1", "arg2");
        var exception = await Assert.ThrowsAsync<ProcessExecutionException>(
            () => process.RunAsync(default));
        Assert.Equal(2, exception.ExitCode);
        Assert.Equal("test arg1 arg2", exception.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_Twice_ThrowTest()
    {
        var process = new TestProcess("test", "arg1", "arg2");
        var exception1 = Assert.Throws<ProcessExecutionException>(process.Run);
        Assert.Equal(2, exception1.ExitCode);
        Assert.Equal("test arg1 arg2", exception1.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);

        var exception2 = Assert.Throws<ProcessExecutionException>(process.Run);
        Assert.Equal(2, exception2.ExitCode);
        Assert.Equal("test arg1 arg2", exception2.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_Twice_ThrowTestAsync()
    {
        var process = new TestProcess("test", "arg1", "arg2");
        var exception1 = await Assert.ThrowsAsync<ProcessExecutionException>(
            () => process.RunAsync(default));
        Assert.Equal(2, exception1.ExitCode);
        Assert.Equal("test arg1 arg2", exception1.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);

        var exception2 = await Assert.ThrowsAsync<ProcessExecutionException>(
            () => process.RunAsync(default));
        Assert.Equal(2, exception2.ExitCode);
        Assert.Equal("test arg1 arg2", exception2.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_Dotnet_ThrowTest()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath);
        var exception = Assert.Throws<ProcessExecutionException>(process.Run);
        Assert.Equal(129, exception.ExitCode);
        Assert.Equal(dotnetPath, exception.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_Dotnet_ThrowTestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath);
        var exception = await Assert.ThrowsAsync<ProcessExecutionException>(
            () => process.RunAsync(default));
        Assert.Equal(129, exception.ExitCode);
        Assert.Equal(dotnetPath, exception.CommandLine);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_DotnetHelp_Test()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        using var outputCollector = new ProcessOutputCollector(process);
        process.Run();
        Assert.NotEmpty(outputCollector.Output);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_DotnetHelp_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        using var outputCollector = new ProcessOutputCollector(process);
        await process.RunAsync(default);
        Assert.NotEmpty(outputCollector.Output);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_DotnetHelp_Twice_Test()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        var outputCollector1 = new ProcessOutputCollector(process);
        process.Run();
        outputCollector1.Dispose();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
        var outputCollector2 = new ProcessOutputCollector(process);
        process.Run();
        outputCollector2.Dispose();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);

        Assert.Equal(outputCollector1.Output, outputCollector2.Output);
    }

    [Fact]
    public async Task RunAsync_DotnetHelp_Twice_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        var outputCollector1 = new ProcessOutputCollector(process);
        await process.RunAsync(default);
        outputCollector1.Dispose();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
        var outputCollector2 = new ProcessOutputCollector(process);
        await process.RunAsync(default);
        outputCollector2.Dispose();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);

        Assert.Equal(outputCollector1.Output, outputCollector2.Output);
    }

    [Fact]
    public void Run_Runtime_Test()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath);
        using var outputCollector = new ProcessOutputCollector(process);
        var expectedTimestamp = DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds(1000);
        process.Run();
        var output = outputCollector.Output;
        var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var actualTimestamp = DateTimeOffset.Parse(lines.Last(), CultureInfo.CurrentCulture);

        Assert.True(actualTimestamp >= expectedTimestamp);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_Runtime_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath);
        using var outputCollector = new ProcessOutputCollector(process);
        var expectedTimestamp = DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds(1000);
        await process.RunAsync(default);
        var output = outputCollector.Output;
        var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var actualTimestamp = DateTimeOffset.Parse(lines.Last(), CultureInfo.CurrentCulture);

        Assert.True(actualTimestamp >= expectedTimestamp);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_Runtime_Timeout_ThrowTest()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath, "1000");
        Assert.Throws<OperationCanceledException>(() => process.Run(10));
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_Runtime_Timeout_ThrowTestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath, "10000");
        using var cancellationTokenSource = new CancellationTokenSource(10);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => process.RunAsync(cancellationTokenSource.Token));
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_Runtime_InvalidArgs_ThrowTest()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath, "abc");
        var exception = Assert.Throws<ProcessExecutionException>(() => process.Run());
        Assert.Equal(134, exception.ExitCode);
        Assert.NotEmpty(exception.Message);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_Runtime_InvalidArgs_ThrowTestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath, "abc");
        var exception = await Assert.ThrowsAsync<ProcessExecutionException>(
            () => process.RunAsync(default));
        Assert.Equal(134, exception.ExitCode);
        Assert.NotEmpty(exception.Message);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_TestId_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var millisecondsDelay = Random.Shared.Next(1000, 2000);
        var process = new TestProcess(dotnetPath, executionPath, $"{millisecondsDelay}");
        using var outputCollector = new ProcessOutputCollector(process);
        var expectedTimestamp
            = DateTimeOffset.UtcNow + TimeSpan.FromMilliseconds(millisecondsDelay);
        var task = process.RunAsync(default);

        Assert.True(process.IsRunning);
        Assert.NotEqual(-1, process.Id);
        await task;

        var output = outputCollector.Output;
        var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var actualTimestamp = DateTimeOffset.Parse(lines.Last(), CultureInfo.CurrentCulture);
        Assert.True(actualTimestamp >= expectedTimestamp);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }
}
