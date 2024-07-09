using LibplanetConsole.Databases.Serializations;

namespace LibplanetConsole.Databases.Services;

public interface IDatabaseCallback
{
    void OnStarted(DatabaseInfo databaseInfo);

    void OnStopped();
}
