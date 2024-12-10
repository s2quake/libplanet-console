namespace LibplanetConsole.Console.Bank;

public interface INodeBank
{
    CurrencyCollection Currencies { get; }

    Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetBalanceAsync(
        Currency currency, CancellationToken cancellationToken);
}
