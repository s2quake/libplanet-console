using OnBoarding.ConsoleHost.Exceptions;

namespace OnBoarding.ConsoleHost;

sealed class Bot(IServiceProvider serviceProvider, User user)
{
    private Task? _task;
    private CancellationTokenSource? _cancellationTokenSource;

    public bool IsRunning { get; private set; }

    public User User => user;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Bot has been started.");

        _cancellationTokenSource = new();
        _task = RunAsync(_cancellationTokenSource.Token);
        await Task.CompletedTask;
        IsRunning = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Bot has been stopped.");

        _cancellationTokenSource!.Cancel();
        _cancellationTokenSource = null;
        await _task!;
        _task = null;
        IsRunning = false;
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var random = new Random(user.GetHashCode());
            var swarmHosts = (SwarmHostCollection)serviceProvider.GetService(typeof(SwarmHostCollection))!;
            while (cancellationToken.IsCancellationRequested == false)
            {
                var v = random.Next(100);
                var tick = random.Next(100, 2000);
                await Task.Delay(tick, cancellationToken);
                var swarmHost = swarmHosts.Current;
                if (user.IsOnline == false && v < 50)
                {
                    user.Login(swarmHost);
                }
                else if (user.IsOnline == true && v < 10)
                {
                    user.Logout();
                }
                else if (user.IsOnline == true && v < 50)
                {
                    if (RandomUtility.GetNext(100) < 90)
                    {
                    }
                    else if (user.PlayerInfo == null)
                    {
                        await user.CreateCharacterAsync(swarmHost, cancellationToken);
                    }
                    else if (user.PlayerInfo.Life <= 0)
                    {
                        await user.ReviveCharacterAsync(swarmHost, cancellationToken);
                    }
                    else
                    {
                        var @out = user.Out;
                        var blockIndex = await user.PlayGameAsync(swarmHost, cancellationToken);
                        user.Out = new StringWriter();
                        await @out.WriteLineAsync("replaying.");
                        await user.ReplayGameAsync(swarmHost, blockIndex, tick: 500, cancellationToken);
                        user.Out = @out;
                        await @out.WriteLineAsync("replayed.");
                    }
                }

            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}
