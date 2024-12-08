namespace LibplanetConsole.Console.Bank;

public interface IBank
{
    CurrencyCollection Currencies { get; }

    Task TransferAsync(
        Address recipientAddress, FungibleAssetValue amount, CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetBalanceAsync(
        Currency currency, CancellationToken cancellationToken);

    Task AllocateAsync(
        Address recipientAddress, FungibleAssetValue amount, CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetPoolAsync(
        Currency currency, CancellationToken cancellationToken);
}
