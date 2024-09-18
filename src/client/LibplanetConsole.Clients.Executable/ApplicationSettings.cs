using System.ComponentModel;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Executable;

[ApplicationSettings(IsRequired = true)]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the EndPoint on which the Client Service will run. " +
                    "If omitted, host is 127.0.0.1 and port is set to random.")]
    [AppEndPoint]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the private key of the client. " +
                    "If omitted, a random private key is used.")]
    [AppPrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    [JsonIgnore]
    [Category("Hidden")]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    [AppEndPoint]
    public string NodeEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The file path to store log.")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    [DefaultValue("")]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the REPL is not started.")]
    [JsonIgnore]
    public bool NoREPL { get; init; }

    public ApplicationOptions ToOptions(object[] components)
    {
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var privateKey = AppPrivateKey.ParseOrRandom(PrivateKey);
        return new ApplicationOptions(endPoint, privateKey)
        {
            ParentProcessId = ParentProcessId,
            NodeEndPoint = AppEndPoint.ParseOrDefault(NodeEndPoint),
            LogPath = GetFullPath(LogPath),
            NoREPL = NoREPL,
            Components = components,
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    public static ApplicationSettings Parse(string[] args)
    {
        var options = new ApplicationSettings();
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, commandSettings);
        parser.Parse(args);
        return options;
    }
}
