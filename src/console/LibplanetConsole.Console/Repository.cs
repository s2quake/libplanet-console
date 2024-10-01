using System.ComponentModel;
using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Extensions;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Console;

public sealed record class Repository
{
    private const int DefaultTimeout = 10000;

    private byte[]? _genesis;

    public Repository(EndPoint endPoint, NodeOptions[] nodes, ClientOptions[] clients)
    {
        EndPoint = endPoint;
        Nodes = nodes;
        Clients = clients;
    }

    public EndPoint EndPoint { get; }

    public NodeOptions[] Nodes { get; } = [];

    public ClientOptions[] Clients { get; } = [];

    public byte[] Genesis
    {
        get => _genesis ??= CreateDefaultGenesis();
        init => _genesis = value;
    }

    public string LogPath { get; init; } = string.Empty;

    public string LibraryLogPath { get; init; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

    public static byte[] CreateGenesis(GenesisOptions genesisOptions)
    {
        var genesisProcess = new NodeGenesisProcess
        {
            GenesisOptions = genesisOptions,
        };

        var genesis = genesisProcess.RunWithResult();
        return ByteUtil.ParseHex(genesis);
    }

    public static Repository Load(string repositoryPath, RepositoryPathResolver resolver)
    {
        if (Path.IsPathRooted(repositoryPath) is false)
        {
            throw new ArgumentException(
                $"'{repositoryPath}' must be an absolute path.", nameof(repositoryPath));
        }

        var nodesPath = resolver.GetNodesPath(repositoryPath);
        var nodeOptions = Directory.GetDirectories(nodesPath)
            .Select(LoadNodeOptions)
            .ToArray();
        var clientsPath = resolver.GetClientsPath(repositoryPath);
        var clientOptions = Directory.GetDirectories(clientsPath)
            .Select(LoadClientOptions)
            .ToArray();
        var applicationSettings = LoadSettings(repositoryPath, resolver);
        var endPoint = EndPointUtility.Parse(applicationSettings.EndPoint);
        var genesisPath = resolver.GetGenesisPath(repositoryPath);

        return new(endPoint, nodeOptions, clientOptions)
        {
            Genesis = LoadGenesis(genesisPath),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, repositoryPath),
            LibraryLogPath = Path.GetFullPath(applicationSettings.LibraryLogPath, repositoryPath),
            Source = repositoryPath,
        };

        NodeOptions LoadNodeOptions(string nodePath)
        {
            var privateKey = new PrivateKey(Path.GetFileName(nodePath));
            var settingsPath = resolver.GetNodeSettingsPath(nodePath, privateKey);
            return NodeOptions.Load(settingsPath);
        }

        ClientOptions LoadClientOptions(string clientPath)
        {
            var privateKey = new PrivateKey(Path.GetFileName(clientPath));
            var settingsPath = resolver.GetClientSettingsPath(clientPath, privateKey);
            return ClientOptions.Load(settingsPath);
        }
    }

    public dynamic Save(string repositoryPath, RepositoryPathResolver resolver)
    {
        if (Path.IsPathRooted(repositoryPath) is false)
        {
            throw new ArgumentException(
                $"'{repositoryPath}' must be an absolute path.", nameof(repositoryPath));
        }

        if (Directory.Exists(repositoryPath) is true
            && Directory.GetFiles(repositoryPath).Length > 0)
        {
            throw new ArgumentException(
                $"'{repositoryPath}' is not empty.", nameof(repositoryPath));
        }

        if (File.Exists(repositoryPath) is true)
        {
            throw new ArgumentException(
                $"'{repositoryPath}' is not a directory.", nameof(repositoryPath));
        }

        PathUtility.EnsureDirectory(repositoryPath);

        dynamic info = new ExpandoObject();
        var settingsPath = resolver.GetSettingsPath(repositoryPath);
        var schemaPath = resolver.GetSettingsSchemaPath(repositoryPath);
        var genesisPath = resolver.GetGenesisPath(repositoryPath);

        info.RepositoryPath = repositoryPath;

        SaveGenesis(genesisPath);
        info.GenesisPath = genesisPath;
        SaveSettingsSchema(schemaPath);
        info.SchemaPath = schemaPath;
        SaveSettings(schemaPath, settingsPath);
        info.SettingsPath = settingsPath;

        info.Nodes = new List<ExpandoObject>(Nodes.Length);
        Array.ForEach(Nodes, node =>
        {
            var genesisPath = resolver.GetGenesisPath(repositoryPath);
            var nodesPath = resolver.GetNodesPath(repositoryPath);
            var nodePath = resolver.GetNodePath(nodesPath, node.PrivateKey);
            var process = new NodeRepositoryProcess
            {
                PrivateKey = node.PrivateKey,
                EndPoint = node.EndPoint,
                OutputPath = nodePath,
                GenesisPath = genesisPath,
                ActionProviderModulePath = node.ActionProviderModulePath,
                ActionProviderType = node.ActionProviderType,
            };
            var sb = new StringBuilder();
            process.OutputDataReceived += (_, e) => sb.AppendLine(e.Data);
            process.Run(DefaultTimeout);
            info.Nodes.Add(JsonUtility.DeserializeSchema<ExpandoObject>(sb.ToString()));
        });

        info.Clients = new List<ExpandoObject>(Clients.Length);
        Array.ForEach(Clients, client =>
        {
            var clientsPath = resolver.GetClientsPath(repositoryPath);
            var clientPath = resolver.GetClientPath(clientsPath, client.PrivateKey);
            var process = new ClientRepositoryProcess
            {
                PrivateKey = client.PrivateKey,
                EndPoint = client.EndPoint,
                OutputPath = clientPath,
            };
            var sb = new StringBuilder();
            process.OutputDataReceived += (_, e) => sb.AppendLine(e.Data);
            process.Run(DefaultTimeout);
            info.Clients.Add(JsonUtility.DeserializeSchema<ExpandoObject>(sb.ToString()));
        });

        return info;
    }

    private byte[] CreateDefaultGenesis()
    {
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = new PrivateKey(),
            Validators = Nodes.Select(item => item.PrivateKey.PublicKey).ToArray(),
            Timestamp = DateTimeOffset.UtcNow,
            ActionProviderModulePath = string.Empty,
            ActionProviderType = string.Empty,
        };

        return CreateGenesis(genesisOptions);
    }

    private static byte[] LoadGenesis(string genesisPath)
    {
        var text = File.ReadAllLines(genesisPath);
        return ByteUtil.ParseHex(text[0]);
    }

    private static ApplicationSettings LoadSettings(
        string repositoryPath, RepositoryPathResolver resolver)
    {
        var settingsPath = resolver.GetSettingsPath(repositoryPath);
        var json = File.ReadAllText(settingsPath);
        var settings = JsonUtility.DeserializeSchema<Settings>(json);
        return settings.Application;
    }

    private static void SaveSettingsSchema(string schemaPath)
    {
        var schemaBuilder = new ApplicationSettingsSchemaBuilder();
        var schema = schemaBuilder.Build();
        File.WriteAllLines(schemaPath, [schema]);
    }

    private void SaveGenesis(string genesisPath)
    {
        var genesis = Genesis;
        var hex = ByteUtil.Hex(genesis);
        File.WriteAllLines(genesisPath, [hex]);
    }

    private void SaveSettings(string schemaPath, string settingsPath)
    {
        var schemaRelativePath = PathUtility.GetRelativePath(settingsPath, schemaPath);
        var settings = new Settings
        {
            Schema = schemaRelativePath,
            Application = new ApplicationSettings
            {
                EndPoint = EndPointUtility.ToString(EndPoint),
                LogPath = LogPath,
                LibraryLogPath = LibraryLogPath,
            },
        };
        var json = JsonUtility.SerializeSchema(settings);
        File.WriteAllLines(settingsPath, [json]);
    }

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public required ApplicationSettings Application { get; init; }
    }

    private sealed record class ApplicationSettings
    {
        public required string EndPoint { get; init; }

        [DefaultValue("")]
        public string LogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LibraryLogPath { get; init; } = string.Empty;
    }
}
