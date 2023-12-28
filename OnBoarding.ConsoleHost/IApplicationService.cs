namespace OnBoarding.ConsoleHost;

interface IApplicationService : IAsyncDisposable
{
    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
