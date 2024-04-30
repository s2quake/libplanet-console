using System.Text;
using Libplanet.Common;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeServices;

public record class NodeOptions
{
    public static NodeOptions Default { get; } = new NodeOptions();

    public GenesisOptions GenesisOptions { get; init; } = GenesisOptions.Default;

    public BoundPeer? SeedPeer { get; init; }

    public BoundPeer? ConsensusSeedPeer { get; init; }

    public override string ToString()
    {
        var s = JsonUtility.SerializeObject((NodeOptionsInfo)this);
        var bytes = Encoding.UTF8.GetBytes(s);
        return ByteUtil.Hex(bytes);
    }

    public static NodeOptions Parse(string text)
    {
        var bytes = ByteUtil.ParseHex(text);
        var s = Encoding.UTF8.GetString(bytes);
        return JsonUtility.DeserializeObject<NodeOptionsInfo>(s);
    }
}
