

using Grpc.Core;
using Grpc.Net.Client;
using Lib9c;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Guild;
using Microsoft.Extensions.DependencyInjection;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Guild;

internal sealed class Guild(IConsole console, INodeCollection nodes, IApplicationOptions options)
    : ConsoleContentBase("guild"), IGuild
{
    private static readonly Codec _codec = new();
    private GrpcChannel? _channel;
    private GuildTxGrpcService.GuildTxGrpcServiceClient? _service;
    private INode? _node;
    private IBlockChain? _blockChain;

    public INode Node => _node ?? throw new InvalidOperationException("Node is not selected.");

    public IBlockChain BlockChain
        => _blockChain ?? throw new InvalidOperationException("Block chain is not selected.");

    public Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var request = new ClaimTxRequest
        {
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.ClaimAsync(request, callOptions);
        var plainValue = _codec.Decode(response.PlainValue.ToByteArray());

        await console.SendTransactionAsync([plainValue], cancellationToken);
    }

    public async Task CreateAsync(
        Address validatorAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        var request = new CreateTxRequest
        {
            ValidatorAddress = ToGrpc(validatorAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.CreateAsync(request, callOptions);
        var plainValue = _codec.Decode(response.PlainValue.ToByteArray());

        await console.SendTransactionAsync([plainValue], cancellationToken);
    }

    public Task DeleteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<GuildInfo> GetGuildAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task LeaveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var index = Random.Shared.Next(nodes.Count);
        var node = nodes[index];
        var address = $"http://{EndPointUtility.ToString(node.EndPoint)}";
        _channel = GrpcChannel.ForAddress(address);
        _service = new GuildTxGrpcService.GuildTxGrpcServiceClient(_channel);
        _node = node;
        _blockChain = node.GetRequiredKeyedService<IBlockChain>(INode.Key);

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _channel = null;

        await Task.CompletedTask;
    }
}
