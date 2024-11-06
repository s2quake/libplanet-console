using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Executable;

public class RepositoryPathResolver
{
    public virtual string GetGenesisPath(string repositoryPath)
        => Path.Combine(repositoryPath, "genesis");

    public virtual string GetAppProtocolVersionPath(string repositoryPath)
        => Path.Combine(repositoryPath, "apvProtocolVersion");

    public virtual string GetSettingsSchemaPath(string repositoryPath)
        => Path.Combine(repositoryPath, "appsettings-schema.json");

    public virtual string GetSettingsPath(string repositoryPath)
        => Path.Combine(repositoryPath, "appsettings.json");

    public virtual string GetNodesPath(string repositoryPath)
        => Path.Combine(repositoryPath, "nodes");

    public virtual string GetNodesTrashPath(string repositoryPath)
        => Path.Combine(repositoryPath, "nodes.trash");

    public virtual string GetNodeSettingsSchemaPath(string nodesPath)
        => Path.Combine(nodesPath, "appsettings-schema.json");

    public virtual string GetNodeSettingsPath(string nodePath)
        => Path.Combine(nodePath, "appsettings.json");

    public virtual string GetNodePath(string nodesPath, Address address)
        => Path.Combine(nodesPath, address.ToString());

    public virtual string GetNodeStorePath(string nodePath)
        => Path.Combine(nodePath, "store");

    public virtual string GetNodeLogPath(string nodePath)
        => Path.Combine(nodePath, "log");

    public virtual string GetClientsPath(string repositoryPath)
        => Path.Combine(repositoryPath, "clients");

    public virtual string GetClientsTrashPath(string repositoryPath)
        => Path.Combine(repositoryPath, "clients.trash");

    public virtual string GetClientSettingsSchemaPath(string clientsPath)
        => Path.Combine(clientsPath, "appsettings-schema.json");

    public virtual string GetClientSettingsPath(string clientPath)
        => Path.Combine(clientPath, "appsettings.json");

    public virtual string GetClientPath(string clientsPath, Address address)
        => Path.Combine(clientsPath, address.ToString());

    public virtual string GetClientLogPath(string clientPath)
        => Path.Combine(clientPath, "log");
}
