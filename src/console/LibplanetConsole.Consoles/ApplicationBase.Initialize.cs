using Bencodex;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public abstract partial class ApplicationBase
{
    public static void Initialize(
        AppPrivateKey genesisKey,
        AppPrivateKey[] nodeKeys,
        AppPrivateKey[] clientKeys,
        string outputPath)
    {
        if (Directory.Exists(outputPath) is true && Directory.GetFiles(outputPath).Length > 0)
        {
            throw new InvalidOperationException($"OutputPath '{outputPath}' is not empty.");
        }

        if (File.Exists(outputPath) is true)
        {
            throw new InvalidOperationException($"OutputPath '{outputPath}' is not a directory.");
        }

        if (Directory.Exists(outputPath) is false)
        {
            Directory.CreateDirectory(outputPath);
        }

        WriteGenesis(genesisKey, nodeKeys, Path.Combine(outputPath, "genesis"));
        WriteNodes(nodeKeys, outputPath);
        WriteNodeSchema(outputPath);
        WriteClients(clientKeys, outputPath);
        WriteClientSchema(outputPath);
    }

    private static void WriteGenesis(
        AppPrivateKey genesisKey, AppPrivateKey[] nodeKeys, string outputPath)
    {
        var validatorKeys = nodeKeys.Select(item => item.PublicKey).ToArray();
        var dateTimeOffset = DateTimeOffset.UtcNow;
        var genesisBlock = BlockUtility.CreateGenesisBlock(
            genesisKey, validatorKeys, dateTimeOffset);

        var blockDictionary = BlockMarshaler.MarshalBlock(genesisBlock);
        var codec = new Codec();
        var bytes = codec.Encode(blockDictionary);
        var hex = ByteUtil.Hex(bytes);
        File.WriteAllText(outputPath, hex);
    }

    private static void WriteNodes(AppPrivateKey[] nodeKeys, string outputPath)
    {
        var nodesPath = Path.Combine(outputPath, "nodes");
        Directory.CreateDirectory(nodesPath);
        foreach (var nodeKey in nodeKeys)
        {
            var nodePath = Path.Combine(nodesPath, ByteUtil.Hex(nodeKey.ToByteArray()));
            var settingsPath = Path.Combine(nodePath, "settings.json");
            Directory.CreateDirectory(nodePath);
            var settings = new Dictionary<string, object>
            {
                ["$schema"] = "../../node-schema.json",
                ["Application"] = new
                {
                    EndPoint = AppEndPoint.Next().ToString(),
                    PrivateKey = ByteUtil.Hex(nodeKey.ToByteArray()),
                    GenesisPath = "../../genesis",
                    StorePath = "./store",
                    LogPath = "./log",
                },
            };
            var json = JsonUtility.Serialize(settings);
            File.WriteAllText(settingsPath, json);
        }
    }

    private static void WriteNodeSchema(string outputPath)
    {
        var schemaPath = Path.Combine(outputPath, "node-schema.json");
        using var process = new NodeSchemaProcess
        {
            OutputPath = schemaPath,
        };
        process.Run(10000);
    }

    private static void WriteClients(AppPrivateKey[] clientKeys, string outputPath)
    {
        var clientsPath = Path.Combine(outputPath, "clients");
        var settingsPath = Path.Combine(clientsPath, "settings.json");
        Directory.CreateDirectory(clientsPath);
        foreach (var clientKey in clientKeys)
        {
            var clientPath = Path.Combine(clientsPath, ByteUtil.Hex(clientKey.ToByteArray()));
            Directory.CreateDirectory(clientPath);
            var settings = new Dictionary<string, string>
            {
                ["$schema"] = "../../client-schema.json",
            };
            var json = JsonUtility.Serialize(settings);
            File.WriteAllText(settingsPath, json);
        }
    }

    private static void WriteClientSchema(string outputPath)
    {
        var schemaPath = Path.Combine(outputPath, "client-schema.json");
        using var process = new ClientSchemaProcess
        {
            OutputPath = schemaPath,
        };
        process.Run(10000);
    }
}
