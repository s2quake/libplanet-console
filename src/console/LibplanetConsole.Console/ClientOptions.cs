using System.ComponentModel;
using System.Diagnostics;
using LibplanetConsole.Common;
using Microsoft.Extensions.Configuration;

namespace LibplanetConsole.Console;

public sealed record class ClientOptions
{
    public required Uri Url { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public Uri? HubUrl { get; init; }

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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(settingsPath)!)
            .AddJsonFile(settingsPath, optional: false, reloadOnChange: true)
            .Build();
        var urls = GetUrls(configuration);
        if (urls.Length == 0)
        {
            throw new ArgumentException("At least one endpoint is required.");
        }

        var url = new Uri(urls[0]);
        var applicationSettings = new ApplicationSettings();
        configuration.Bind("Application", applicationSettings);

        return new()
        {
            Url = url,
            PrivateKey = new PrivateKey(applicationSettings.PrivateKey),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, repositoryPath),
            HubUrl = UriUtility.ParseOrDefault(applicationSettings.NodeUrl),
            RepositoryPath = repositoryPath,
        };
    }

    private static string[] GetUrls(IConfiguration configuration)
    {
        var kestrelSection = configuration.GetSection("Kestrel");
        var endpointsSection = kestrelSection.GetSection("Endpoints");

        var urlList = new List<string>();
        foreach (var item in endpointsSection.GetChildren())
        {
            var urlSection = item.GetSection("Url");
            var url = urlSection.Value ?? throw new UnreachableException("Url is required.");
            urlList.Add(url);
        }

        return [.. urlList];
    }

    private sealed record class ApplicationSettings
    {
        public string PrivateKey { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string NodeUrl { get; init; } = string.Empty;
    }
}
