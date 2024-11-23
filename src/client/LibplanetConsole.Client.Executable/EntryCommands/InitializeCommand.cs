using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Client.Executable.EntryCommands;

[CommandSummary("Creates a repository to run the libplanet-client")]
internal sealed class InitializeCommand : CommandBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("Specifies the directory path to create repository")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty)]
    public string OutputPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the private key of the client.")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the port of the client.")]
    [NonNegative]
    public int Port { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the file path to save logs")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch("quiet", 'q')]
    [CommandSummary("If set, the command does not output any information")]
    public bool Quiet { get; set; }

    protected override void OnExecute()
    {
        var outputPath = Path.GetFullPath(OutputPath);
        var port = Port == 0 ? PortUtility.NextPort() : Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var logPath = Path.Combine(outputPath, LogPath.Fallback("log"));
        var repository = new Repository
        {
            Port = port,
            PrivateKey = privateKey,
            LogPath = logPath,
        };
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };
        var info = repository.Save(outputPath);

        TextWriterExtensions.WriteLineAsJson(writer, info);
    }
}
