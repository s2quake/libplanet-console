using System.ComponentModel;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console;

public sealed record class ClientOptions
{
    public required EndPoint EndPoint { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public EndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    internal string RepositoryPath { get; private set; } = string.Empty;

    public static ClientOptions Load(string settingsPath)
    {
        if (Path.IsPathRooted(settingsPath) is false)
        {
            throw new ArgumentException(
                $"'{settingsPath}' must be an absolute path.", nameof(settingsPath));
        }

        var repositoryPath = Path.GetDirectoryName(settingsPath)
            ?? throw new ArgumentException("Invalid settings file path.", nameof(settingsPath));
        var json = File.ReadAllText(settingsPath);
        var settings = JsonUtility.DeserializeSchema<Settings>(json);
        var applicationSettings = settings.Application;

        return new()
        {
            EndPoint = GetLocalHost(applicationSettings.Port),
            PrivateKey = new PrivateKey(applicationSettings.PrivateKey),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, repositoryPath),
            NodeEndPoint = ParseOrDefault(applicationSettings.NodeEndPoint),
            RepositoryPath = repositoryPath,
        };
    }

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public required ApplicationSettings Application { get; init; } = new();
    }

    private sealed record class ApplicationSettings
    {
        public int Port { get; init; } = 0;

        public string PrivateKey { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string NodeEndPoint { get; init; } = string.Empty;
    }
}
