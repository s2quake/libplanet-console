using Grpc.Core;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Grpc.Bank;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Bank;

internal sealed class Bank(IServiceProvider serviceProvider, IClient client)
    : GrpcClientContentBase<BankGrpcService.BankGrpcServiceClient>(client, "bank"), IBank
{
    public async Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken)
    {
        var currencies = serviceProvider.GetRequiredService<ICurrencyCollection>();
        var request = new TransferRequest
        {
            RecipientAddress = ToGrpc(recipientAddress),
            Amount = currencies.ToString(amount),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.TransferAsync(request, callOptions);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken)
    {
        var currencies = serviceProvider.GetRequiredService<ICurrencyCollection>();
        var request = new GetBalanceRequest
        {
            Address = ToGrpc(address),
            Currency = currencies.GetCode(currency),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetBalanceAsync(request, callOptions);
        return currencies.ToFungibleAssetValue(response.Balance);
    }

    public async Task<CurrencyInfo[]> GetCurrenciesAsync(CancellationToken cancellationToken)
    {
        var request = new GetCurrenciesRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetCurrenciesAsync(request, callOptions);
        return response.Currencies.Select(currency => new CurrencyInfo
        {
            Code = currency.Code,
            Currency = new Currency(ToIValue(currency.Currency)),
        }).ToArray();
    }
}
