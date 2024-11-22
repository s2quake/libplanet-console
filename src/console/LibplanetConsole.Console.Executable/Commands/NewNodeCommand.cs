using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Console.Executable.Commands;

[CommandSummary("Creates a new node")]
internal sealed class NewNodeCommand(
    NodeCommand nodeCommand,
    Application application,
    IApplicationOptions options,
    INodeCollection nodes)
    : CommandAsyncBase(nodeCommand, "new")
{
    [CommandProperty]
    [CommandSummary("Indicates the private key of the node. " +
                    "If omitted, a random private key is used.")]
    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The port of the node. " +
                    "If omitted, a random endpoint is used.")]
    [NonNegative]
    public int Port { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The node instance is created, but not actually started")]
    public bool NoProcess { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The console does not attach to the target process after the node process " +
                    "is started.\n" +
                    "This option cannot be used with --no-process option.")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The node process is started in a new window.\n" +
                    "This option cannot be used with --no-process option.")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool NewWindow { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var port = Port == 0 ? PortUtility.NextPort() : Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var nodeOptions = application.RepositoryPath != string.Empty
            ? await CreateNodeRepositoryAsync(
                application.RepositoryPath, privateKey, port, cancellationToken)
            : new NodeOptions
            {
                EndPoint = EndPointUtility.GetLocalHost(port),
                PrivateKey = privateKey,
                ActionProviderModulePath = options.ActionProviderModulePath,
                ActionProviderType = options.ActionProviderType,
            };
        var addNewOptions = new AddNewNodeOptions
        {
            NodeOptions = nodeOptions,
            ProcessOptions = NoProcess is false
                ? new ProcessOptions { Detach = Detach, NewWindow = NewWindow, }
                : null,
        };
        await nodes.AddNewAsync(addNewOptions, cancellationToken);
    }

    private async Task<NodeOptions> CreateNodeRepositoryAsync(
        string repositoryPath, PrivateKey privateKey, int port, CancellationToken cancellationToken)
    {
        var oldDirectory = Directory.GetCurrentDirectory();
        var resolver = new RepositoryPathResolver();
        try
        {
            var nodesPath = resolver.GetNodesPath(repositoryPath);
            var nodePath = resolver.GetNodePath(nodesPath, privateKey.Address);
            var nodeSettingsPath = resolver.GetNodeSettingsPath(nodePath);
            var process = new NodeRepositoryProcess
            {
                PrivateKey = privateKey,
                Port = port,
                GenesisPath = resolver.GetGenesisPath(repositoryPath),
                AppProtocolVersionPath = resolver.GetAppProtocolVersionPath(repositoryPath),
                OutputPath = nodePath,
                ActionProviderModulePath = options.ActionProviderModulePath,
                ActionProviderType = options.ActionProviderType,
            };
            await process.RunAsync(cancellationToken);

            return NodeOptions.Load(nodeSettingsPath);
        }
        finally
        {
            Directory.SetCurrentDirectory(oldDirectory);
        }
    }
}
