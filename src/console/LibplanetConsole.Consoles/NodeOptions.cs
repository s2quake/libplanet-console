using System.ComponentModel;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public sealed record class NodeOptions
{
    public required AppEndPoint EndPoint { get; init; }

    public required AppPrivateKey PrivateKey { get; init; }

    public AppEndPoint? SeedEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; init; } = string.Empty;

    public string LibraryLogPath { get; init; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

    public static NodeOptions Load(string settingsPath)
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
            StorePath = Path.GetFullPath(applicationSettings.StorePath, directoryName),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, directoryName),
            LibraryLogPath = Path.GetFullPath(applicationSettings.LibraryLogPath, directoryName),
            Source = settingsPath,
            SeedEndPoint = AppEndPoint.ParseOrDefault(applicationSettings.SeedEndPoint),
        };
    }

    private sealed record class Settings
    {
        [JsonPropertyName("$schema")]
        public required string Schema { get; init; } = string.Empty;

        public ApplicationSettings Application { get; init; } = new();
    }

    private sealed record class ApplicationSettings
    {
        public string EndPoint { get; init; } = string.Empty;

        public string PrivateKey { get; init; } = string.Empty;

        [DefaultValue("")]
        public string GenesisPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string StorePath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LibraryLogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string SeedEndPoint { get; init; } = string.Empty;
    }
}
