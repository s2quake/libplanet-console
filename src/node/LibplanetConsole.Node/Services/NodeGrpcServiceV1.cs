using Grpc.Core;
using LibplanetConsole.Node.Grpc;

namespace LibplanetConsole.Node.Services;

public sealed class NodeGrpcServiceV1(INode node) : NodeGrpcService.NodeGrpcServiceBase
{
    public override async Task<StartResponse> Start(StartRequest request, ServerCallContext context)
    {
        await node.StartAsync(context.CancellationToken);
        return new StartResponse { NodeInfo = node.Info };
    }

    public async override Task<StopResponse> Stop(StopRequest request, ServerCallContext context)
    {
        await node.StopAsync(context.CancellationToken);
        return new StopResponse();
    }

    public override Task<GetInfoResponse> GetInfo(GetInfoRequest request, ServerCallContext context)
    {
        GetInfoResponse Action() => new()
        {
            NodeInfo = node.Info,
        };

        return Task.Run(Action, context.CancellationToken);
    }
}
