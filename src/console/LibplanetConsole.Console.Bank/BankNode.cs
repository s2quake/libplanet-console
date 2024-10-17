namespace LibplanetConsole.Console.Bank;

internal sealed class BankNode(INode node) : NodeContentBase("bank-node"), IBank
{
    public Task<FungibleAssetValue> BurnAsync(Address address, FungibleAssetValue amount, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FungibleAssetValue> GetBalanceAsync(Address address, Currency currency, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FungibleAssetValue> MintAsync(Address address, FungibleAssetValue amount, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<FungibleAssetValue> TransferAsync(Address address, Address targetAddress, FungibleAssetValue amount, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
