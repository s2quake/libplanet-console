namespace LibplanetConsole.Node.Executable;

internal sealed class AddressProvider(IApplicationOptions options) : IAddressProvider
{
    public IEnumerable<AddressInfo> AddressInfos
    {
        get
        {
            if (options.Alias != string.Empty)
            {
                yield return new AddressInfo
                {
                    Alias = options.Alias,
                    Address = options.PrivateKey.Address,
                    Tags = ["custom"],
                };
            }
        }
    }
}
