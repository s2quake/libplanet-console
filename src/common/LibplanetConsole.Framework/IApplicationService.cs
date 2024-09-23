namespace LibplanetConsole.Framework;

public interface IApplicationService
{
    Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
