using System.ComponentModel;
using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;
using Libplanet.Common;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

public sealed record class Repository
{
    private const int DefaultTimeout = 10000;

    public Repository(AppEndPoint endPoint, NodeOptions[] nodes, ClientOptions[] clients)
    {
        EndPoint = endPoint;
        Nodes = nodes;
        Clients = clients;
        Genesis = BlockUtility.SerializeBlock(BlockUtility.CreateGenesisBlock(
            genesisKey: new AppPrivateKey(),
            validatorKeys: nodes.Select(item => item.PrivateKey.PublicKey).ToArray(),
            dateTimeOffset: DateTimeOffset.UtcNow));
    }

    public AppEndPoint EndPoint { get; }

    public NodeOptions[] Nodes { get; } = [];

    public ClientOptions[] Clients { get; } = [];

    public byte[] Genesis { get; init; } = [];

    public string LogPath { get; init; } = string.Empty;

    public string LibraryLogPath { get; init; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

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
        var endPoint = AppEndPoint.Parse(applicationSettings.EndPoint);
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
            var privateKey = AppPrivateKey.Parse(Path.GetFileName(nodePath));
            var settingsPath = resolver.GetNodeSettingsPath(nodePath, privateKey);
            return NodeOptions.Load(settingsPath);
        }

        ClientOptions LoadClientOptions(string clientPath)
        {
            var privateKey = AppPrivateKey.Parse(Path.GetFileName(clientPath));
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
            };
            var sb = new StringBuilder();
            process.OutputDataReceived += (_, e) => sb.AppendLine(e.Data);
            process.Run(DefaultTimeout);
            info.Nodes.Add(JsonUtility.Deserialize<ExpandoObject>(sb.ToString()));
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
            info.Clients.Add(JsonUtility.Deserialize<ExpandoObject>(sb.ToString()));
        });

        return info;
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
        var settings = JsonUtility.Deserialize<Settings>(json);
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
                EndPoint = EndPoint.ToString(),
                LogPath = LogPath,
                LibraryLogPath = LibraryLogPath,
            },
        };
        var json = JsonUtility.Serialize(settings);
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
