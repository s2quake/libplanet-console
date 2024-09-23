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
        process.Run();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_DotnetHelp_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        await process.RunAsync(default);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_DotnetHelp_Twice_Test()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        process.Run();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
        process.Run();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_DotnetHelp_Twice_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var process = new TestProcess(dotnetPath, "--help");
        await process.RunAsync(default);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
        await process.RunAsync(default);
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public void Run_Runtime_Test()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath);
        process.Run();
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }

    [Fact]
    public async Task RunAsync_Runtime_TestAsync()
    {
        var dotnetPath = ProcessEnvironment.DotnetPath;
        var executionPath = consoleApplicationFixture.ExecutionPath;
        var process = new TestProcess(dotnetPath, executionPath);
        await process.RunAsync(default);
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
        var process = new TestProcess(dotnetPath, executionPath, "1000");
        var task = process.RunAsync(default);
        Assert.True(process.IsRunning);
        Assert.NotEqual(-1, process.Id);
        await task;
        Assert.False(process.IsRunning);
        Assert.Equal(-1, process.Id);
    }
}
