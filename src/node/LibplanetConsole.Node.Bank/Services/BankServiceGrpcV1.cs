using Grpc.Core;
using LibplanetConsole.Grpc.Bank;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Bank.Services;

internal sealed class BankServiceGrpcV1(Bank bank)
    : BankGrpcService.BankGrpcServiceBase
{
    public override async Task<MintResponse> Mint(MintRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var amount = ToFungibleAssetValue(request.Amount);
        var balance = await bank.MintAsync(address, amount, context.CancellationToken);
        return new MintResponse { Balance = ToGrpc(balance) };
    }

    public override async Task<TransferResponse> Transfer(
        TransferRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var targetAddress = ToAddress(request.TargetAddress);
        var amount = ToFungibleAssetValue(request.Amount);
        var balance = await bank.TransferAsync(
            address, targetAddress, amount, context.CancellationToken);
        return new TransferResponse { Balance = ToGrpc(balance) };
    }

    public override async Task<BurnResponse> Burn(BurnRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var amount = ToFungibleAssetValue(request.Amount);
        var balance = await bank.BurnAsync(address, amount, context.CancellationToken);
        return new BurnResponse { Balance = ToGrpc(balance) };
    }

    public override async Task<GetBalanceResponse> GetBalance(
        GetBalanceRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var currency = ToCurrency(request.Currency);
        var balance = await bank.GetBalanceAsync(address, currency, context.CancellationToken);
        return new GetBalanceResponse { Balance = ToGrpc(balance) };
    }
}
