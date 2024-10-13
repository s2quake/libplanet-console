using System.ComponentModel;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Client.Executable;

[ApplicationSettings(IsRequired = true)]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the port on which the client will run. " +
                    "If omitted, a random port is used.")]
    [NonNegative]
    public int Port { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the private key of the client. " +
                    "If omitted, a random private key is used.")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    [JsonIgnore]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    [EndPoint]
    public string NodeEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the file path to save logs.")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    [DefaultValue("")]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the client runs without REPL.")]
    [JsonIgnore]
    public bool NoREPL { get; init; }

    public ApplicationOptions ToOptions()
    {
        var port = Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        return new ApplicationOptions(port, privateKey)
        {
            ParentProcessId = ParentProcessId,
            NodeEndPoint = EndPointUtility.ParseOrDefault(NodeEndPoint),
            LogPath = GetFullPath(LogPath),
            NoREPL = NoREPL,
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
