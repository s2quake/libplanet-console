using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Executable.Commands;

[CommandSummary("Deletes a client")]
internal sealed class DeleteClientCommand(
    ClientCommand clientCommand, Application application, IClientCollection clients)
    : CommandAsyncBase(clientCommand, "delete")
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the address of the client")]
    public Address Address { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var client = clients[Address];
        await client.DisposeAsync();

        if (application.RepositoryPath != string.Empty)
        {
            var repositoryPath = application.RepositoryPath;
            var resolver = new RepositoryPathResolver();
            var clientAddress = client.Address;
            var clientsPath = resolver.GetClientsPath(repositoryPath);
            var trashPath = resolver.GetClientsTrashPath(repositoryPath);
            var clientPath1 = resolver.GetClientPath(clientsPath, clientAddress);
            var clientPath2 = resolver.GetClientPath(trashPath, clientAddress);
            PathUtility.EnsureDirectory(trashPath);
            Directory.Move(clientPath1, clientPath2);
        }
    }
}
