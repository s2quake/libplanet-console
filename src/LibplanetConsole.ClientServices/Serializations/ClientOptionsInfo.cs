using JSSoft.Communication;

namespace LibplanetConsole.ClientServices.Serializations;

public record struct ClientOptionsInfo
{
    public string NodeEndPoint { get; set; }

    public static implicit operator ClientOptions(ClientOptionsInfo info)
    {
        return new ClientOptions
        {
            NodeEndPoint = EndPointUtility.Parse(info.NodeEndPoint),
        };
    }

    public static implicit operator ClientOptionsInfo(ClientOptions clientOptions)
    {
        return new ClientOptionsInfo
        {
            NodeEndPoint = EndPointUtility.ToString(clientOptions.NodeEndPoint),
        };
    }
}
