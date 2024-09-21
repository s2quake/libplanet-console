using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Progresses;
using LibplanetConsole.Console.Extensions;
using LibplanetConsole.Options;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console.Executable;

public sealed record class Repository
{
    private const int DefaultTimeout = 10000;
    private const int BlinkOfAnEye = 300;

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

    public async Task<dynamic> SaveAsync(
        string repositoryPath,
        RepositoryPathResolver resolver,
        CancellationToken cancellationToken,
        IProgress<ProgressInfo> progress)
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
        var stepProgress = progress.Step(0.0, 1.0, 3 + Nodes.Length + Clients.Length);

        info.RepositoryPath = repositoryPath;

        stepProgress.Next("Saving the genesis...");
        await SaveGenesisAsync(genesisPath, cancellationToken);
        info.GenesisPath = genesisPath;
        await SaveAppProtocolVersionAsync(appProtocolVersionPath, cancellationToken);
        info.AppProtocolVersionPath = appProtocolVersionPath;
        stepProgress.Next("Saving the schema...");
        await SaveSettingsSchemaAsync(schemaPath, cancellationToken);
        info.SchemaPath = schemaPath;
        stepProgress.Next("Saving the settings...");
        await SaveSettingsAsync(
            schemaPath, settingsPath, applicationOptions, kestrelOptions, cancellationToken);
        info.SettingsPath = settingsPath;

        info.Nodes = new List<ExpandoObject>(Nodes.Length);
        PathUtility.EnsureDirectory(nodesPath);
        for (var i = 0; i < Nodes.Length; i++)
        {
            stepProgress.Next($"Initializing node {i}...");
            var node = Nodes[i];
            var process = new NodeRepositoryProcess
            {
                PrivateKey = node.PrivateKey,
                Port = GetPort(node.EndPoint),
                OutputPath = resolver.GetNodePath(nodesPath, node.PrivateKey),
                GenesisPath = resolver.GetGenesisPath(repositoryPath),
                AppProtocolVersionPath = appProtocolVersionPath,
                ActionProviderModulePath = node.ActionProviderModulePath,
                ActionProviderType = node.ActionProviderType,
            };
            var sb = new StringBuilder();
            using var processCancellationTokenSource = new CancellationTokenSource(DefaultTimeout);
            using var linkedCancellationTokenSource
                = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken, processCancellationTokenSource.Token);
            process.OutputDataReceived += (_, e) => sb.AppendLine(e.Data);
            await process.RunAsync(linkedCancellationTokenSource.Token);
            info.Nodes.Add(JsonUtility.DeserializeSchema<ExpandoObject>(sb.ToString()));
        }

        info.Clients = new List<ExpandoObject>(Clients.Length);
        PathUtility.EnsureDirectory(clientsPath);
        for (var i = 0; i < Clients.Length; i++)
        {
            stepProgress.Next($"Initializing client {i}...");
            var client = Clients[i];
            var process = new ClientRepositoryProcess
            {
                PrivateKey = client.PrivateKey,
                Port = GetPort(client.EndPoint),
                OutputPath = resolver.GetClientPath(clientsPath, client.PrivateKey),
            };
            var sb = new StringBuilder();
            using var processCancellationTokenSource = new CancellationTokenSource(DefaultTimeout);
            using var linkedCancellationTokenSource
                = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken, processCancellationTokenSource.Token);
            process.OutputDataReceived += (_, e) => sb.AppendLine(e.Data);
            await process.RunAsync(linkedCancellationTokenSource.Token);
            info.Clients.Add(JsonUtility.DeserializeSchema<ExpandoObject>(sb.ToString()));
        }

        await Task.Delay(1000, default);
        stepProgress.Complete("Done.");
        await Task.Delay(BlinkOfAnEye, default);

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

    private static async Task SaveSettingsSchemaAsync(
        string schemaPath, CancellationToken cancellationToken)
    {
        var schemaBuilder = OptionsSchemaBuilder.Create();
        var schema = schemaBuilder.Build();
        await File.WriteAllLinesAsync(schemaPath, [schema], cancellationToken);
    }

    private async Task SaveGenesisAsync(string genesisPath, CancellationToken cancellationToken)
    {
        var genesis = Genesis;
        var hex = ByteUtil.Hex(genesis);
        await File.WriteAllLinesAsync(genesisPath, [hex], cancellationToken);
    }

    private async Task SaveAppProtocolVersionAsync(
        string appProtocolVersionPath, CancellationToken cancellationToken)
    {
        var appProtocolVersion = AppProtocolVersion;
        var hex = appProtocolVersion.Token;
        await File.WriteAllLinesAsync(appProtocolVersionPath, [hex], cancellationToken);
    }

    private static async Task SaveSettingsAsync(
        string schemaPath,
        string settingsPath,
        ApplicationOptions applicationOptions,
        dynamic kestrelOptions,
        CancellationToken cancellationToken)
    {
        var schemaRelativePath = PathUtility.GetRelativePath(settingsPath, schemaPath);
        var settings = new Settings
        {
            Schema = schemaRelativePath,
            Application = applicationOptions,
            Kestrel = kestrelOptions,
        };
        var json = JsonUtility.SerializeSchema(settings);
        await File.WriteAllLinesAsync(settingsPath, [json], cancellationToken);
    }

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public required ApplicationOptions Application { get; init; }

        public required dynamic Kestrel { get; init; }
    }
}
