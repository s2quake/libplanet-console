namespace LibplanetConsole.Node;

public interface IAddressProvider
{
    IEnumerable<AddressInfo> AddressInfos { get; }
}
