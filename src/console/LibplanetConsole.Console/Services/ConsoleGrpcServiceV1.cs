using Grpc.Core;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Grpc;
using LibplanetConsole.Grpc;
using LibplanetConsole.Node;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LibplanetConsole.Console.Services;

public sealed class ConsoleGrpcServiceV1(
    IServer server,
    IApplicationOptions options,
    INodeCollection nodes,
    IClientCollection clients)
    : ConsoleGrpcService.ConsoleGrpcServiceBase
{
    private EndPoint? _seedEndPoint;

    public override Task<GetNodeSettingsResponse> GetNodeSettings(
        GetNodeSettingsRequest request, ServerCallContext context)
    {
        var genesis = BlockUtility.SerializeBlock(options.GenesisBlock);
        return Task.FromResult(new GetNodeSettingsResponse
        {
            AppProtocolVersion = options.AppProtocolVersion,
            Genesis = TypeUtility.ToGrpc(genesis),
            ProcessId = Environment.ProcessId,
            SeedEndPoint = EndPointUtility.ToString(GetSeedEndPoint()),
        });
    }

    public override Task<GetClientSettingsResponse> GetClientSettings(
        GetClientSettingsRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetClientSettingsResponse
        {
            ProcessId = Environment.ProcessId,
            NodeEndPoint = EndPointUtility.ToString(RandomNodeEndPoint()),
        });
    }

    public override async Task<AttachNodeResponse> AttachNode(
        AttachNodeRequest request, ServerCallContext context)
    {
        var attachOptions = new AttachOptions
        {
            Address = TypeUtility.ToAddress(request.Address),
            EndPoint = EndPointUtility.Parse(request.EndPoint),
            ProcessId = request.ProcessId,
        };
        await nodes.AttachAsync(attachOptions, context.CancellationToken);
        return new AttachNodeResponse();
    }

    public override async Task<AttachClientResponse> AttachClient(
        AttachClientRequest request, ServerCallContext context)
    {
        var attachOptions = new AttachOptions
        {
            Address = TypeUtility.ToAddress(request.Address),
            EndPoint = EndPointUtility.Parse(request.EndPoint),
            ProcessId = request.ProcessId,
        };
        await clients.AttachAsync(attachOptions, context.CancellationToken);
        return new AttachClientResponse();
    }

    private EndPoint GetSeedEndPoint()
    {
        if (_seedEndPoint is null)
        {
            var addressesFeature = server.Features.Get<IServerAddressesFeature>()
                ?? throw new InvalidOperationException("ServerAddressesFeature is not available.");
            var address = addressesFeature.Addresses.First();
            var url = new Uri(address);

            _seedEndPoint = new DnsEndPoint(url.Host, url.Port);
        }

        return _seedEndPoint;
    }

    private EndPoint RandomNodeEndPoint()
    {
        var nodeIndex = Random.Shared.Next(nodes.Count);
        var node = nodes[nodeIndex];
        return node.EndPoint;
    }
}
