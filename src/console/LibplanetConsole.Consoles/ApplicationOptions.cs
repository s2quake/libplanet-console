using Libplanet.Common;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(string path, AppEndPoint endPoint)
    {
        RepositoryPath = path;
        EndPoint = endPoint;
    }

    public string RepositoryPath { get; }

    public AppEndPoint EndPoint { get; }

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }

    public bool ManualStart { get; init; }

    public object[] Components { get; init; } = [];

    internal AppPrivateKey[] GetNodeKeys()
    {
        var nodesPath = Path.Combine(RepositoryPath, "nodes");
        return File.ReadAllLines(nodesPath)
            .Select(AppPrivateKey.Parse)
            .ToArray();
    }

    internal AppPrivateKey[] GetClientKeys()
    {
        var clientsPath = Path.Combine(RepositoryPath, "clients");
        return File.ReadAllLines(clientsPath)
            .Select(AppPrivateKey.Parse)
            .ToArray();
    }

    internal Block GetGenesisBlock()
    {
        var genesisPath = System.IO.Path.Combine(RepositoryPath, "genesis");
        var hex = File.ReadAllText(genesisPath);
        return BlockUtility.DeserializeBlock(ByteUtil.ParseHex(hex));
    }
}
