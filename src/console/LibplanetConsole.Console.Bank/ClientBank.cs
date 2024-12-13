using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Bank;
using Microsoft.Extensions.DependencyInjection;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Bank;

internal sealed class ClientBank(
    [FromKeyedServices(IClient.Key)] IClient client,
    ICurrencyCollection currencies)
    : ClientContentBase("client-bank"), IClientBank
{
    private GrpcChannel? _channel;
    private BankGrpcService.BankGrpcServiceClient? _service;

    public async Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var request = new TransferRequest
        {
            RecipientAddress = ToGrpc(recipientAddress),
            Amount = currencies.ToString(amount),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.TransferAsync(request, callOptions);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var request = new GetBalanceRequest
        {
            Address = ToGrpc(address),
            Currency = currencies.GetCode(currency),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetBalanceAsync(request, callOptions);
        return currencies.ToFungibleAssetValue(response.Balance);
    }

    public async Task<CurrencyInfo[]> GetCurrenciesAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var request = new GetCurrenciesRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetCurrenciesAsync(request, callOptions);
        return response.Currencies.Select(currency => new CurrencyInfo
        {
            Code = currency.Code,
            Currency = new Currency(ToIValue(currency.Currency)),
        }).ToArray();
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var address = $"http://{EndPointUtility.ToString(client.EndPoint)}";
        _channel = GrpcChannel.ForAddress(address);
        _service = new BankGrpcService.BankGrpcServiceClient(_channel);

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _channel = null;

        await Task.CompletedTask;
    }
}
