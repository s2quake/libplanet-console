using System.Dynamic;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using LibplanetConsole.Options;
using static LibplanetConsole.Common.PathUtility;

namespace LibplanetConsole.Client.Executable;

public sealed record class Repository
{
    public const string SettingsFileName = "appsettings.json";
    public const string SettingsSchemaFileName = "appsettings-schema.json";

    public required int Port { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public EndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public dynamic Save(string repositoryPath)
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

        var privateKey = PrivateKey;
        var settingsPath = Path.Combine(repositoryPath, SettingsFileName);
        var schemaPath = Path.Combine(repositoryPath, SettingsSchemaFileName);
        var schemaBuilder = OptionsSchemaBuilder.Create();
        var schema = schemaBuilder.Build();

        EnsureDirectory(repositoryPath);

        File.WriteAllLines(schemaPath, [schema]);
        var settings = new Settings
        {
            Schema = SettingsSchemaFileName,
            Application = new ApplicationOptions
            {
                PrivateKey = PrivateKeyUtility.ToString(privateKey),
                LogPath = GetRelativePathFromDirectory(repositoryPath, LogPath),
                NodeEndPoint = EndPointUtility.ToString(NodeEndPoint),
            },
            Kestrel = new
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
            },
        };

        var json = JsonUtility.SerializeSchema(settings);
        File.WriteAllLines(settingsPath, [json]);

        dynamic info = new ExpandoObject();
        info.RepositoryPath = repositoryPath;
        info.SchemaPath = schemaPath;
        info.SettingsPath = settingsPath;
        return info;
    }

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public required ApplicationOptions Application { get; init; }

        public required dynamic Kestrel { get; init; }
    }
}
