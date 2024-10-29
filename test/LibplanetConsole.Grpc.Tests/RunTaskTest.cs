using LibplanetConsole.Common.Threading;

namespace LibplanetConsole.Grpc.Tests;

public class RunTaskTest
{
    [Fact]
    public async Task RunAsync()
    {
        using var runTask = new RunTask();
        var expectedDateTime = DateTime.UtcNow.AddSeconds(RunTask.DefaultSeconds);
        await runTask.RunAsync(default);

        var actualDateTime = DateTime.UtcNow;
        Assert.False(runTask.IsRunning);
        Assert.True(actualDateTime - expectedDateTime < TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task RunAsync_Twice_ThrowAsync()
    {
        using var runTask = new RunTask();
        var task = runTask.RunAsync(default);
        await Assert.ThrowsAsync<InvalidOperationException>(() => runTask.RunAsync(default));
        await TaskUtility.TryWait(task);
        Assert.True(runTask.IsRunning);
    }

    [Fact(Timeout = 3000)]
    public async Task RunAsync_Cancel_ThrowAsync()
    {
        using var runTask = new RunTask() { TimeSpan = TimeSpan.FromSeconds(10) };
        using var cancellationTokenSource = new CancellationTokenSource(millisecondsDelay: 1000);
        await Assert.ThrowsAsync<TaskCanceledException>(
            async () => await runTask.RunAsync(cancellationTokenSource.Token));
        Assert.False(runTask.IsRunning);
    }

    [Fact]
    public async Task StartAsync()
    {
        using var runTask = new RunTask();
        await runTask.StartAsync(default);
        Assert.True(runTask.IsRunning);
    }

    [Fact]
    public async Task StopAsync()
    {
        using var runTask = new RunTask();
        await runTask.StartAsync(default);
        await runTask.StopAsync(default);
        Assert.False(runTask.IsRunning);
    }

    private sealed class RunTask : RunTaskBase
    {
        public const int DefaultSeconds = 1;

        public TimeSpan TimeSpan { get; set; } = TimeSpan.FromSeconds(DefaultSeconds);

        protected override async Task OnRunAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan, cancellationToken);
        }
    }
}
