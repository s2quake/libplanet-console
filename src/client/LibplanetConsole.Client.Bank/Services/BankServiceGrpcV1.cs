using Grpc.Core;
using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Bank.Services;

internal sealed class BankServiceGrpcV1(IBank bank, ICurrencyCollection currencies)
    : BankGrpcService.BankGrpcServiceBase
{
    public override async Task<TransferResponse> Transfer(
        TransferRequest request, ServerCallContext context)
    {
        var recipientAddress = ToAddress(request.RecipientAddress);
        var amount = currencies.ToFungibleAssetValue(request.Amount);
        await bank.TransferAsync(
            recipientAddress, amount, context.CancellationToken);
        return new TransferResponse { };
    }

    public override async Task<GetBalanceResponse> GetBalance(
        GetBalanceRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var currency = currencies[request.Currency];
        var balance = await bank.GetBalanceAsync(address, currency, context.CancellationToken);
        return new GetBalanceResponse { Balance = currencies.ToString(balance) };
    }

    public override Task<GetCurrenciesResponse> GetCurrencies(
        GetCurrenciesRequest request, ServerCallContext context)
    {
        var infos = currencies.Codes.Select(item => new CurrencyInfoProto
        {
            Code = item,
            Currency = ToGrpc(currencies[item].Serialize()),
        });

        return Task.FromResult(new GetCurrenciesResponse { Currencies = { infos } });
    }
}
