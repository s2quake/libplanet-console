using System.Dynamic;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using LibplanetConsole.Framework;
using static LibplanetConsole.Common.PathUtility;

namespace LibplanetConsole.Client.Executable;

public sealed record class Repository
{
    public const string SettingsFileName = "client-settings.json";
    public const string SettingsSchemaFileName = "client-settings-schema.json";

    public required int Port { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public EndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public static Repository Load(string settingsPath)
    {
        if (Path.IsPathRooted(settingsPath) is false)
        {
            throw new ArgumentException(
                $"'{settingsPath}' must be an absolute path.", nameof(settingsPath));
        }

        var directoryName = Path.GetDirectoryName(settingsPath)
            ?? throw new ArgumentException("Invalid settings file path.", nameof(settingsPath));
        var json = File.ReadAllText(settingsPath);
        var settings = JsonUtility.DeserializeSchema<Settings>(json);
        var applicationSettings = settings.Application;

        return new()
        {
            Port = applicationSettings.Port,
            PrivateKey = new PrivateKey(applicationSettings.PrivateKey),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, directoryName),
            NodeEndPoint = EndPointUtility.ParseOrDefault(applicationSettings.NodeEndPoint),
        };
    }

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
        var schemaBuilder = new ApplicationSettingsSchemaBuilder();
        var schema = schemaBuilder.Build();

        EnsureDirectory(repositoryPath);

        File.WriteAllLines(schemaPath, [schema]);
        var settings = new Settings
        {
            Schema = SettingsSchemaFileName,
            Application = new ApplicationSettings
            {
                Port = Port,
                PrivateKey = PrivateKeyUtility.ToString(privateKey),
                LogPath = GetRelativePathFromDirectory(repositoryPath, LogPath),
                NodeEndPoint = EndPointUtility.ToString(NodeEndPoint),
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

        public required ApplicationSettings Application { get; init; }
    }
}
