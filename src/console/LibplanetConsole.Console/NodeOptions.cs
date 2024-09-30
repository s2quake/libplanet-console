using System.ComponentModel;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

public sealed record class NodeOptions
{
    public required EndPoint EndPoint { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public EndPoint? SeedEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; init; } = string.Empty;

    public string LibraryLogPath { get; init; } = string.Empty;

    internal string RepositoryPath { get; private set; } = string.Empty;

    public static NodeOptions Load(string settingsPath)
    {
        if (Path.IsPathRooted(settingsPath) is false)
        {
            throw new ArgumentException(
                $"'{settingsPath}' must be an absolute path.", nameof(settingsPath));
        }

        var repositoryPath = Path.GetDirectoryName(settingsPath)
            ?? throw new ArgumentException("Invalid settings file path.", nameof(settingsPath));
        var json = File.ReadAllText(settingsPath);
        var settings = JsonUtility.Deserialize<Settings>(json);
        var applicationSettings = settings.Application;

        return new()
        {
            EndPoint = EndPointUtility.Parse(applicationSettings.EndPoint),
            PrivateKey = new PrivateKey(applicationSettings.PrivateKey),
            StorePath = Path.GetFullPath(applicationSettings.StorePath, repositoryPath),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, repositoryPath),
            LibraryLogPath = Path.GetFullPath(applicationSettings.LibraryLogPath, repositoryPath),
            SeedEndPoint = EndPointUtility.ParseOrDefault(applicationSettings.SeedEndPoint),
            RepositoryPath = repositoryPath,
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
