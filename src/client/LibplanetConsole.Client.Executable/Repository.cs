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

    public required AppEndPoint EndPoint { get; init; }

    public required AppPrivateKey PrivateKey { get; init; }

    public AppEndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

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
        var settings = JsonUtility.Deserialize<Settings>(json);
        var applicationSettings = settings.Application;

        return new()
        {
            EndPoint = AppEndPoint.Parse(applicationSettings.EndPoint),
            PrivateKey = AppPrivateKey.Parse(applicationSettings.PrivateKey),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, directoryName),
            Source = settingsPath,
            NodeEndPoint = AppEndPoint.ParseOrDefault(applicationSettings.NodeEndPoint),
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
                EndPoint = EndPoint.ToString(),
                PrivateKey = AppPrivateKey.ToString(privateKey),
                LogPath = GetRelativePathFromDirectory(repositoryPath, LogPath),
                NodeEndPoint = AppEndPoint.ToString(NodeEndPoint),
            },
        };

        var json = JsonUtility.Serialize(settings);
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
