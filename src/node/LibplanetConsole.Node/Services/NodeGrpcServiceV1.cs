using Grpc.Core;
using LibplanetConsole.Node.Grpc;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Services;

internal sealed class NodeGrpcServiceV1(
    IHostApplicationLifetime applicationLifetime, INode node)
    : NodeGrpcService.NodeGrpcServiceBase
{
    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse());
    }

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

    public override async Task GetStartedStream(
        GetStartedStreamRequest request,
        IServerStreamWriter<GetStartedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetStartedStreamResponse>(
            responseStream,
            handler => node.Started += handler,
            handler => node.Started -= handler,
            () => new GetStartedStreamResponse { NodeInfo = node.Info });
        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }

    public override async Task GetStoppedStream(
        GetStoppedStreamRequest request,
        IServerStreamWriter<GetStoppedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetStoppedStreamResponse>(
            responseStream,
            handler => node.Stopped += handler,
            handler => node.Stopped -= handler);
        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }
}
