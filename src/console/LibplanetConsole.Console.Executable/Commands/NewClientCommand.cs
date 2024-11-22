using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Console.Executable.Commands;

[CommandSummary("Creates a new client")]
internal sealed class NewClientCommand(
    ClientCommand clientCommand,
    Application application,
    IClientCollection clients)
    : CommandAsyncBase(clientCommand, "new")
{
    [CommandProperty]
    [CommandSummary("Indicates the private key of the client. " +
                    "If omitted, a random private key is used.")]
    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The port of the client. " +
                    "If omitted, a random endpoint is used.")]
    [NonNegative]
    public int Port { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The client instance is created, but not actually started")]
    public bool NoProcess { get; set; }

    [CommandSummary("The console does not attach to the target process after the client process " +
                    "is started.\n" +
                    "This option cannot be used with --no-process option.")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The client process is started in a new window.\n" +
                    "This option cannot be used with --no-process option.")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool NewWindow { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var port = Port == 0 ? PortUtility.NextPort() : Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var clientOptions = application.RepositoryPath != string.Empty
            ? await CreateClientRepositoryAsync(
                application.RepositoryPath, privateKey, port, cancellationToken)
            : new ClientOptions
            {
                EndPoint = EndPointUtility.GetLocalHost(port),
                PrivateKey = privateKey,
            };
        var addNewOptions = new AddNewClientOptions
        {
            ClientOptions = clientOptions,
            ProcessOptions = NoProcess is false
                ? new ProcessOptions { Detach = Detach, NewWindow = NewWindow, }
                : null,
        };
        await clients.AddNewAsync(addNewOptions, cancellationToken);
    }

    private async Task<ClientOptions> CreateClientRepositoryAsync(
        string repositoryPath, PrivateKey privateKey, int port, CancellationToken cancellationToken)
    {
        var oldDirectory = Directory.GetCurrentDirectory();
        var resolver = new RepositoryPathResolver();
        try
        {
            var clientsPath = resolver.GetClientsPath(repositoryPath);
            var clientPath = resolver.GetClientPath(clientsPath, privateKey.Address);
            var clientSettingsPath = resolver.GetClientSettingsPath(clientPath);
            var process = new ClientRepositoryProcess
            {
                PrivateKey = privateKey,
                Port = port,
                OutputPath = clientPath,
            };
            await process.RunAsync(cancellationToken);
            return ClientOptions.Load(clientSettingsPath);
        }
        finally
        {
            Directory.SetCurrentDirectory(oldDirectory);
        }
    }
}
