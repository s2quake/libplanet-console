using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

public class RepositoryPathResolver
{
    public virtual string GetGenesisPath(string repositoryPath)
        => Path.Combine(repositoryPath, "genesis");

    public virtual string GetSettingsSchemaPath(string repositoryPath)
        => Path.Combine(repositoryPath, "settings-schema.json");

    public virtual string GetSettingsPath(string repositoryPath)
        => Path.Combine(repositoryPath, "settings.json");

    public virtual string GetNodesPath(string repositoryPath)
        => Path.Combine(repositoryPath, "nodes");

    public virtual string GetNodeSettingsSchemaPath(string nodesPath)
        => Path.Combine(nodesPath, "node-settings-schema.json");

    public virtual string GetNodeSettingsPath(string nodePath, PrivateKey privateKey)
        => Path.Combine(nodePath, "node-settings.json");

    public virtual string GetNodePath(string nodesPath, PrivateKey privateKey)
        => Path.Combine(nodesPath, PrivateKeyUtility.ToString(privateKey));

    public virtual string GetNodeStorePath(string nodePath, PrivateKey privateKey)
        => Path.Combine(nodePath, "store");

    public virtual string GetNodeLogPath(string nodePath, PrivateKey privateKey)
        => Path.Combine(nodePath, "log");

    public virtual string GetClientsPath(string repositoryPath)
        => Path.Combine(repositoryPath, "clients");

    public virtual string GetClientSettingsSchemaPath(string clientsPath)
        => Path.Combine(clientsPath, "client-settings-schema.json");

    public virtual string GetClientSettingsPath(string clientPath, PrivateKey privateKey)
        => Path.Combine(clientPath, "client-settings.json");

    public virtual string GetClientPath(string clientsPath, PrivateKey privateKey)
        => Path.Combine(clientsPath, PrivateKeyUtility.ToString(privateKey));

    public virtual string GetClientLogPath(string clientPath, PrivateKey privateKey)
        => Path.Combine(clientPath, "log");
}
