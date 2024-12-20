using System.ComponentModel;
using System.Diagnostics;
using LibplanetConsole.Common;
using Microsoft.Extensions.Configuration;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console;

public sealed record class NodeOptions
{
    public required EndPoint EndPoint { get; init; }

    public required PrivateKey PrivateKey { get; init; }

    public EndPoint? SeedEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; init; } = string.Empty;

    public string ActionProviderModulePath { get; init; } = string.Empty;

    public string ActionProviderType { get; init; } = string.Empty;

    public int BlocksyncPort { get; init; }

    public int ConsensusPort { get; init; }

    public string Alias { get; set; } = string.Empty;

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
            EndPoint = new DnsEndPoint(url.Host, url.Port),
            PrivateKey = new PrivateKey(applicationSettings.PrivateKey),
            StorePath = Path.GetFullPath(applicationSettings.StorePath, repositoryPath),
            LogPath = Path.GetFullPath(applicationSettings.LogPath, repositoryPath),
            SeedEndPoint = ParseOrDefault(applicationSettings.SeedEndPoint),
            RepositoryPath = repositoryPath,
            ActionProviderModulePath = applicationSettings.ActionProviderModulePath,
            ActionProviderType = applicationSettings.ActionProviderType,
            BlocksyncPort = applicationSettings.BlocksyncPort == 0
                ? PortUtility.NextPort()
                : applicationSettings.BlocksyncPort,
            ConsensusPort = applicationSettings.ConsensusPort == 0
                ? PortUtility.NextPort()
                : applicationSettings.ConsensusPort,
            Alias = applicationSettings.Alias,
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
        public string GenesisPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string StorePath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string LogPath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string SeedEndPoint { get; init; } = string.Empty;

        [DefaultValue("")]
        public string ActionProviderModulePath { get; init; } = string.Empty;

        [DefaultValue("")]
        public string ActionProviderType { get; init; } = string.Empty;

        [DefaultValue(0)]
        public int BlocksyncPort { get; init; } = 0;

        [DefaultValue(0)]
        public int ConsensusPort { get; init; } = 0;

        [DefaultValue("")]
        public string Alias { get; init; } = string.Empty;
    }
}
