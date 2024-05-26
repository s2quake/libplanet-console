namespace LibplanetConsole.Frameworks;

public interface IApplicationService
{
    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
