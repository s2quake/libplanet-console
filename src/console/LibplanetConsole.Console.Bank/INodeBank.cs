namespace LibplanetConsole.Console.Bank;

public interface INodeBank
{
    Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken);
}
