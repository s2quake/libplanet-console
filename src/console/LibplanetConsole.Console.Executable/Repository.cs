using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Extensions;
using LibplanetConsole.Options;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console.Executable;

public sealed record class Repository
{
    private const int DefaultTimeout = 10000;

    private byte[]? _genesis;
    private AppProtocolVersion? _appProtocolVersion;

    public Repository(int port, NodeOptions[] nodes, ClientOptions[] clients)
    {
        if (port <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(port), port, "Port must be greater than 0.");
        }

        Port = port;
        Nodes = nodes;
        Clients = clients;
    }

    public int Port { get; }

    public NodeOptions[] Nodes { get; } = [];

    public ClientOptions[] Clients { get; } = [];

    public byte[] Genesis
    {
        get => _genesis ??= CreateDefaultGenesis();
        init => _genesis = value;
    }

    public AppProtocolVersion AppProtocolVersion
    {
        get => _appProtocolVersion ??= CreateAppProtocolVersion(new(), 1, string.Empty);
        init => _appProtocolVersion = value;
    }

    public string LogPath { get; init; } = string.Empty;

    public static byte[] CreateGenesis(GenesisOptions genesisOptions)
    {
        var genesisProcess = new NodeGenesisProcess
        {
            GenesisOptions = genesisOptions,
        };

        var genesis = genesisProcess.RunWithResult();
        return ByteUtil.ParseHex(genesis);
    }

    public static AppProtocolVersion CreateAppProtocolVersion(
        PrivateKey privateKey, int version, string extra)
    {
        var process = new NodeAppProtocolVersionProcess
        {
            PrivateKey = privateKey,
            Version = version,
            Extra = extra,
        };

        var token = process.RunWithResult();
        return AppProtocolVersion.FromToken(token);
    }

    public static NodeOptions[] LoadNodeOptions(
        string repositoryPath, RepositoryPathResolver resolver)
    {
        if (Path.IsPathRooted(repositoryPath) is false)
        {
            throw new ArgumentException(
                $"'{repositoryPath}' must be an absolute path.", nameof(repositoryPath));
        }

        var nodesPath = resolver.GetNodesPath(repositoryPath);
        return Directory.GetDirectories(nodesPath)
            .Select(LoadNodeOptions)
            .ToArray();

        NodeOptions LoadNodeOptions(string nodePath)
        {
            var privateKey = new PrivateKey(Path.GetFileName(nodePath));
            var settingsPath = resolver.GetNodeSettingsPath(nodePath, privateKey);
            return NodeOptions.Load(settingsPath);
        }
    }

    public static ClientOptions[] LoadClientOptions(
        string repositoryPath, RepositoryPathResolver resolver)
    {
        if (Path.IsPathRooted(repositoryPath) is false)
        {
            throw new ArgumentException(
                $"'{repositoryPath}' must be an absolute path.", nameof(repositoryPath));
        }

        var clientsPath = resolver.GetClientsPath(repositoryPath);
        return Directory.GetDirectories(clientsPath)
            .Select(LoadClientOptions)
            .ToArray();

        ClientOptions LoadClientOptions(string clientPath)
        {
            var privateKey = new PrivateKey(Path.GetFileName(clientPath));
            var settingsPath = resolver.GetClientSettingsPath(clientPath, privateKey);
            return ClientOptions.Load(settingsPath);
        }
    }

    public byte[] CreateGenesis(
        PrivateKey genesisKey,
        DateTimeOffset dateTimeOffset)
    {
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = genesisKey,
            Validators = Nodes.Select(item => item.PrivateKey.PublicKey).ToArray(),
            Timestamp = dateTimeOffset,
        };

        return CreateGenesis(genesisOptions);
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
        var appProtocolVersionPath = resolver.GetAppProtocolVersionPath(repositoryPath);
        var nodesPath = resolver.GetNodesPath(repositoryPath);
        var clientsPath = resolver.GetClientsPath(repositoryPath);
        var applicationOptions = new ApplicationOptions
        {
            GenesisPath = PathUtility.GetRelativePath(settingsPath, genesisPath),
            AppProtocolVersionPath = PathUtility.GetRelativePath(
                settingsPath, appProtocolVersionPath),
            LogPath = LogPath,
        };
        var kestrelOptions = new
        {
            Endpoints = new
            {
                Http1 = new
                {
                    Url = $"http://localhost:{Port}",
                    Protocols = "Http2",
                },
                Http1AndHttp2 = new
                {
                    Url = $"http://localhost:{Port + 1}",
                    Protocols = "Http1AndHttp2",
                },
            },
        };

        info.RepositoryPath = repositoryPath;

        SaveGenesis(genesisPath);
        info.GenesisPath = genesisPath;
        SaveAppProtocolVersion(appProtocolVersionPath);
        info.AppProtocolVersionPath = appProtocolVersionPath;
        SaveSettingsSchema(schemaPath);
        info.SchemaPath = schemaPath;
        SaveSettings(schemaPath, settingsPath, applicationOptions, kestrelOptions);
        info.SettingsPath = settingsPath;

        info.Nodes = new List<ExpandoObject>(Nodes.Length);
        PathUtility.EnsureDirectory(nodesPath);
        Array.ForEach(Nodes, node =>
        {
            var genesisPath = resolver.GetGenesisPath(repositoryPath);
            var nodePath = resolver.GetNodePath(nodesPath, node.PrivateKey);
            var process = new NodeRepositoryProcess
            {
                PrivateKey = node.PrivateKey,
                Port = GetPort(node.EndPoint),
                OutputPath = nodePath,
                GenesisPath = genesisPath,
                AppProtocolVersionPath = appProtocolVersionPath,
                ActionProviderModulePath = node.ActionProviderModulePath,
                ActionProviderType = node.ActionProviderType,
            };
            var sb = new StringBuilder();
            process.OutputDataReceived += (_, e) => sb.AppendLine(e.Data);
            process.Run(DefaultTimeout);
            info.Nodes.Add(JsonUtility.DeserializeSchema<ExpandoObject>(sb.ToString()));
        });

        info.Clients = new List<ExpandoObject>(Clients.Length);
        PathUtility.EnsureDirectory(clientsPath);
        Array.ForEach(Clients, client =>
        {
            var clientPath = resolver.GetClientPath(clientsPath, client.PrivateKey);
            var process = new ClientRepositoryProcess
            {
                PrivateKey = client.PrivateKey,
                Port = GetPort(client.EndPoint),
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

    private static void SaveSettingsSchema(string schemaPath)
    {
        var schemaBuilder = OptionsSchemaBuilder.Create();
        var schema = schemaBuilder.Build();
        File.WriteAllLines(schemaPath, [schema]);
    }

    private void SaveGenesis(string genesisPath)
    {
        var genesis = Genesis;
        var hex = ByteUtil.Hex(genesis);
        File.WriteAllLines(genesisPath, [hex]);
    }

    private void SaveAppProtocolVersion(string appProtocolVersionPath)
    {
        var appProtocolVersion = AppProtocolVersion;
        var hex = appProtocolVersion.Token;
        File.WriteAllLines(appProtocolVersionPath, [hex]);
    }

    private static void SaveSettings(
        string schemaPath,
        string settingsPath,
        ApplicationOptions applicationOptions,
        dynamic kestrelOptions)
    {
        var schemaRelativePath = PathUtility.GetRelativePath(settingsPath, schemaPath);
        var settings = new Settings
        {
            Schema = schemaRelativePath,
            Application = applicationOptions,
            Kestrel = kestrelOptions,
        };
        var json = JsonUtility.SerializeSchema(settings);
        File.WriteAllLines(settingsPath, [json]);
    }

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public required ApplicationOptions Application { get; init; }

        public required dynamic Kestrel { get; init; }
    }
}
