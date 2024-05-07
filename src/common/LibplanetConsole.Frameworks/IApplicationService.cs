namespace LibplanetConsole.Frameworks;

public interface IApplicationService : IAsyncDisposable
{
    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
