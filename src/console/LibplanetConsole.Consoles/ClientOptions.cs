using System.ComponentModel;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public sealed record class ClientOptions
{
    public required AppEndPoint EndPoint { get; init; }

    public required AppPrivateKey PrivateKey { get; init; }

    public AppEndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

    public static ClientOptions Load(string settingsPath)
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

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public required ApplicationSettings Application { get; init; } = new();
    }

    private sealed record class ApplicationSettings
    {
        public string EndPoint { get; init; } = string.Empty;

        public string PrivateKey { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string NodeEndPoint { get; init; } = string.Empty;
    }
}
