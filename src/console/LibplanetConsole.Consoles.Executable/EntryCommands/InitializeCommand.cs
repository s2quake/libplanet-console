using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;

namespace LibplanetConsole.Consoles.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console.")]
internal sealed class InitializeCommand : CommandBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("The directory path to initialize.")]
    public string OutputPath { get; set; } = string.Empty;

    [CommandProperty]
    [AppPrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty(InitValue = 4)]
    [CommandSummary("The number of nodes to run.\n" +
                    "If omitted, the default value is 4.\n" +
                    "If --nodes option is set, this option is ignored.")]
    [CommandPropertyCondition(nameof(Nodes), null, OnSet = true)]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to run.\n" +
                    "Example: --nodes \"key1,key2,...\"")]
    [CommandPropertyCondition(nameof(NodeCount), null, OnSet = true)]
    [AppPrivateKeyArray]
    public string[] Nodes { get; init; } = [];

    [CommandProperty(InitValue = 2)]
    [CommandSummary("The number of clients to run.\n" +
                    "If omitted, the default value is 2.\n" +
                    "If --clients option is set, this option is ignored.")]
    [CommandPropertyCondition(nameof(Clients), null, OnSet = true)]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to run.\n" +
                    "Example: --clients \"key1,key2,...\"")]
    [CommandPropertyCondition(nameof(ClientCount), null, OnSet = true)]
    [AppPrivateKeyArray]
    public string[] Clients { get; init; } = [];

    protected override void OnExecute()
    {
        var genesisKey = GenesisKey != string.Empty
            ? AppPrivateKey.Parse(GenesisKey) : new AppPrivateKey();
        var nodeKeys = GetNodes();
        var clientKeys = GetClients();
        var outputPath = OutputPath;
        ApplicationBase.Initialize(genesisKey, nodeKeys, clientKeys, outputPath);
    }

    private AppPrivateKey[] GetNodes()
    {
        if (Nodes.Length > 0)
        {
            return [.. Nodes.Select(item => AppPrivateKey.Parse(item))];
        }

        return [.. Enumerable.Range(0, NodeCount).Select(item => new AppPrivateKey())];
    }

    private AppPrivateKey[] GetClients()
    {
        if (Clients.Length > 0)
        {
            return [.. Clients.Select(item => AppPrivateKey.Parse(item))];
        }

        return [.. Enumerable.Range(0, ClientCount).Select(item => new AppPrivateKey())];
    }
}
