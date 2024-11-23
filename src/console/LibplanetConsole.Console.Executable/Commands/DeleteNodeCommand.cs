using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Executable.Commands;

[CommandSummary("Deletes a node")]
internal sealed class DeleteNodeCommand(
    NodeCommand nodeCommand, Application application, INodeCollection nodes)
    : CommandAsyncBase(nodeCommand, "delete")
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the address of the node")]
    public Address Address { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var node = nodes[Address];
        await node.DisposeAsync();

        if (application.RepositoryPath != string.Empty)
        {
            var repositoryPath = application.RepositoryPath;
            var resolver = new RepositoryPathResolver();
            var nodeAddress = node.Address;
            var nodesPath = resolver.GetNodesPath(repositoryPath);
            var trashPath = resolver.GetNodesTrashPath(repositoryPath);
            var nodePath1 = resolver.GetNodePath(nodesPath, nodeAddress);
            var nodePath2 = resolver.GetNodePath(trashPath, nodeAddress);
            PathUtility.EnsureDirectory(trashPath);
            Directory.Move(nodePath1, nodePath2);
        }
    }
}
