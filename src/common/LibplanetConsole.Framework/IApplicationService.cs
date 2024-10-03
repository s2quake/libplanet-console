namespace LibplanetConsole.Framework;

public interface IApplicationService
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
