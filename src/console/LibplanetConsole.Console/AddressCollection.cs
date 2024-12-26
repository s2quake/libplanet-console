namespace LibplanetConsole.Console;

internal sealed class AddressCollection : AddressCollectionBase, IConsoleContent
{
    private readonly IBlockChain _blockChain;
    private readonly Address _consoleAddress;

    public AddressCollection(IBlockChain blockChain, IApplicationOptions options)
    {
        _blockChain = blockChain;
        _consoleAddress = options.PrivateKey.Address;
        _blockChain.Started += BlockChain_Started;
        _blockChain.Stopped += BlockChain_Stopped;
    }

    IEnumerable<IConsoleContent> IConsoleContent.Dependencies { get; } = [];

    string IConsoleContent.Name => "addresses";

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async void BlockChain_Started(object? sender, EventArgs e)
    {
        var addressInfos = await _blockChain.GetAddressesAsync(default);
        foreach (var addressInfo in addressInfos)
        {
            Add(addressInfo);
        }

        Add(new AddressInfo
        {
            Alias = "console",
            Address = _consoleAddress,
        });
    }

    private void BlockChain_Stopped(object? sender, EventArgs e)
    {
        Clear();
    }
}
