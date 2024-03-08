namespace LibplanetConsole.Executable;

interface IApplicationService : IAsyncDisposable
{
    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
