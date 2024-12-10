using Grpc.Core;
using LibplanetConsole.Grpc.Bank;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Bank.Services;

internal sealed class BankServiceGrpcV1(IBank bank)
    : BankGrpcService.BankGrpcServiceBase
{
    public override async Task<TransferResponse> Transfer(
        TransferRequest request, ServerCallContext context)
    {
        var currencies = bank.Currencies;
        var recipientAddress = ToAddress(request.RecipientAddress);
        var amount = currencies.ToFungibleAssetValue(request.Amount);
        await bank.TransferAsync(
            recipientAddress, amount, context.CancellationToken);
        return new TransferResponse { };
    }

    public override async Task<GetBalanceResponse> GetBalance(
        GetBalanceRequest request, ServerCallContext context)
    {
        var currencies = bank.Currencies;
        var currency = currencies[request.Currency];
        var balance = await bank.GetBalanceAsync(currency, context.CancellationToken);
        return new GetBalanceResponse { Balance = currencies.ToString(balance) };
    }

    public override async Task<GetCurrenciesResponse> GetCurrencies(
        GetCurrenciesRequest request, ServerCallContext context)
    {
        var currencies = await Task.FromResult(bank.Currencies);
        var currencyInfos = currencies.GetCurrencyInfos();
        return new GetCurrenciesResponse
        {
            Currencies = { currencyInfos.Select(item => (CurrencyInfoProto)item), },
        };
    }
}
