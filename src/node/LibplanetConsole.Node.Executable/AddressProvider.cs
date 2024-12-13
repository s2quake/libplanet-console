namespace LibplanetConsole.Node.Executable;

internal sealed class AddressProvider : IAddressProvider
{
    public IEnumerable<AddressInfo> Addresses
    {
        get
        {
            yield return new AddressInfo { Alias = "default", Address = default };
        }
    }
}
