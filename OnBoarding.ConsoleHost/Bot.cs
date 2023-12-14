namespace OnBoarding.ConsoleHost;

sealed class Bot
{
    private readonly IServiceProvider _serviceProvider;
    private readonly User _user;
    private Task? _task;
    private CancellationTokenSource? _cancellationTokenSource;

    public Bot(IServiceProvider serviceProvider, User user)
    {
        _serviceProvider = serviceProvider;
        _user = user;
    }

    public bool IsRunning { get; private set; }

    public User User => _user;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == true)
            throw new InvalidOperationException("Bot has been started.");

        _cancellationTokenSource = new();
        _task = RunAsync(_cancellationTokenSource.Token);
        await Task.CompletedTask;
        IsRunning = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == false)
            throw new InvalidOperationException("Bot has been stopped.");

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
            var random = new Random(_user.GetHashCode());
            var swarmHosts = (SwarmHostCollection)_serviceProvider.GetService(typeof(SwarmHostCollection))!;
            while (cancellationToken.IsCancellationRequested == false)
            {
                var v = random.Next(100);
                var tick = random.Next(100, 2000);
                await Task.Delay(tick, cancellationToken);
                var swarmHost = swarmHosts.Current;
                if (_user.IsOnline == false && v < 50)
                {
                    _user.Login(swarmHost);
                }
                else if (_user.IsOnline == true && v < 10)
                {
                    _user.Logout();
                }
                else if (_user.IsOnline == true && v < 50)
                {
                    if (_user.PlayerInfo == null)
                        await _user.CreateCharacterAsync(swarmHost, cancellationToken);
                    else
                        await _user.PlayGameAsync(swarmHost, cancellationToken);
                }

            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}
