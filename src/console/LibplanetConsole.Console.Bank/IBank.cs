
namespace LibplanetConsole.Console.Bank;

public interface IBank
{
    Task<FungibleAssetValue> MintAsync(
        Address address, FungibleAssetValue amount, CancellationToken cancellationToken);

    Task<FungibleAssetValue> TransferAsync(
        Address address,
        Address targetAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken);

    Task<FungibleAssetValue> BurnAsync(
        Address address, FungibleAssetValue amount, CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken);
}
