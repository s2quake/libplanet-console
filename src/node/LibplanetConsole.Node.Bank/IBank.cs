namespace LibplanetConsole.Node.Bank;

public interface IBank
{
    Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken);
}
