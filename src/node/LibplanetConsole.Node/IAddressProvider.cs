namespace LibplanetConsole.Node;

public interface IAddressProvider
{
    IEnumerable<AddressInfo> Addresses { get; }
}
