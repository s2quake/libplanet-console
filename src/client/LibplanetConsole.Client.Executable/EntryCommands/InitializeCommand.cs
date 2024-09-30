using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Client.Executable.EntryCommands;

[CommandSummary("Create a repository to run a libplanet-client")]
internal sealed class InitializeCommand : CommandBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("The directory path to create repository.")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty)]
    public string OutputPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the private key of the client. " +
                    "If omitted, a random private key is used.")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The endpoint of the client. " +
                    "If omitted, a random endpoint is used.")]
    [EndPoint]
    public string EndPoint { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The file path to store the application logs." +
                    "If omitted, the 'app.log' file is used.")]
    [Path(Type = PathType.File, ExistsType = PathExistsType.NotExistOrEmpty, AllowEmpty = true)]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch("quiet", 'q')]
    [CommandSummary("If set, the command does not output any information.")]
    public bool Quiet { get; set; }

    protected override void OnExecute()
    {
        var outputPath = Path.GetFullPath(OutputPath);
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var logPath = Path.Combine(outputPath, LogPath.Fallback("app.log"));
        var repository = new Repository
        {
            EndPoint = endPoint,
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
