namespace LibplanetConsole.Client;

internal sealed class AddressCollection : AddressCollectionBase, IClientContent
{
    private readonly IBlockChain _blockChain;
    private readonly IApplicationOptions _options;

    public AddressCollection(IBlockChain blockChain, IApplicationOptions options)
    {
        _blockChain = blockChain;
        _options = options;
        _blockChain.Started += BlockChain_Started;
        _blockChain.Stopped += BlockChain_Stopped;
    }

    IEnumerable<IClientContent> IClientContent.Dependencies { get; } = [];

    string IClientContent.Name => "addresses";

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async void BlockChain_Started(object? sender, EventArgs e)
    {
        var addressInfos = await _blockChain.GetAddressesAsync(default);
        foreach (var addressInfo in addressInfos)
        {
            Add(addressInfo);
        }

        if (_options.Alias != string.Empty)
        {
            Add(new() { Alias = _options.Alias, Address = _options.PrivateKey.Address });
        }
    }

    private void BlockChain_Stopped(object? sender, EventArgs e)
    {
        Clear();
    }
}
