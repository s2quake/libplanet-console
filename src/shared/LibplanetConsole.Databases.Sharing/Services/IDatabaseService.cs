using LibplanetConsole.Databases.Serializations;

namespace LibplanetConsole.Databases.Services;

public interface IDatabaseService
{
    Task<DatabaseInfo> StartAsync(DatabaseOptions options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<DatabaseInfo> GetInfoAsync(CancellationToken cancellationToken);
}
