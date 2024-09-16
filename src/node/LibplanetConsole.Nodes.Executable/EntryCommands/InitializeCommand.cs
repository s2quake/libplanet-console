using System.Dynamic;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

[CommandSummary("Create a repository to run a libplanet-node")]
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
    [CommandSummary("Indicates the private key of the node. " +
                    "If omitted, a random private key is used.")]
    [AppPrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty]
    [AppEndPoint]
    public string EndPoint { get; set; } = string.Empty;

    [CommandProperty]
    public string StorePath { get; set; } = string.Empty;

    [CommandProperty]
    public string LogPath { get; set; } = string.Empty;

    [CommandProperty]
    public string LibraryLogPath { get; set; } = string.Empty;

    [CommandProperty]
    public string GenesisPath { get; set; } = string.Empty;

    [CommandPropertySwitch]
    public bool NoGenesis { get; set; }

    [CommandPropertySwitch("quiet", 'q')]
    public bool Quiet { get; set; }

    protected override void OnExecute()
    {
        var outputPath = Path.GetFullPath(OutputPath);
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var privateKey = AppPrivateKey.ParseOrRandom(PrivateKey);
        var storePath = Path.Combine(outputPath, StorePath.Fallback("store"));
        var logPath = Path.Combine(outputPath, LogPath.Fallback("app.log"));
        var libraryLogPath = Path.Combine(outputPath, LibraryLogPath.Fallback("library.log"));
        var genesisPath = Path.Combine(outputPath, GenesisPath.Fallback("genesis"));
        var repository = new Repository
        {
            EndPoint = endPoint,
            PrivateKey = privateKey,
            StorePath = storePath,
            LogPath = logPath,
            LibraryLogPath = libraryLogPath,
            GenesisPath = genesisPath,
        };
        dynamic info = repository.Save(outputPath);
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };

        if (NoGenesis is false)
        {
            var genesisKey = privateKey;
            var validatorKeys = new AppPublicKey[] { privateKey.PublicKey };
            var dateTimeOffset = DateTimeOffset.UtcNow;
            var genesis = BlockUtility.CreateGenesisString(
                genesisKey, validatorKeys, dateTimeOffset);
            File.WriteAllLines(genesisPath, [genesis]);
            info.GenesisArguments = new
            {
                GenesisKey = AppPrivateKey.ToString(genesisKey),
                Validators = validatorKeys,
                Timestamp = dateTimeOffset,
            };
            info.Genesis = genesis;
        }

        TextWriterExtensions.WriteLineAsJson(writer, info);
    }
}
