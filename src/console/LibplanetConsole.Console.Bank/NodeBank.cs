using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Bank;
using Microsoft.Extensions.DependencyInjection;
using Nekoyume.Model.State;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Bank;

internal sealed class NodeBank([FromKeyedServices(INode.Key)] INode node)
    : NodeContentBase("node-bank"), INodeBank
{
    private GrpcChannel? _channel;
    private BankGrpcService.BankGrpcServiceClient? _service;

    public CurrencyCollection Currencies { get; private set; } = CurrencyCollection.Empty;

    public async Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        string memo,
        CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var request = new TransferRequest
        {
            RecipientAddress = ToGrpc(recipientAddress),
            Amount = Currencies.ToString(amount),
            Memo = memo,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.TransferAsync(request, callOptions);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Currency currency, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var currencies = Currencies;
        var request = new GetBalanceRequest
        {
            Address = ToGrpc(node.Address),
            Currency = currencies.GetCurrencyAliase(currency),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetBalanceAsync(request, callOptions);
        return currencies.ToFungibleAssetValue(response.Balance);
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var address = $"http://{EndPointUtility.ToString(node.EndPoint)}";
        _channel = GrpcChannel.ForAddress(address);
        _service = new BankGrpcService.BankGrpcServiceClient(_channel);

        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetCurrenciesAsync(new(), callOptions);
        var currencyInfos = response.Currencies.Select(item => (CurrencyInfo)item).ToArray();
        Currencies = new CurrencyCollection(currencyInfos);
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        Currencies = CurrencyCollection.Empty;
        _channel?.Dispose();
        _channel = null;

        await Task.CompletedTask;
    }
}
