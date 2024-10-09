using Grpc.Core;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Node.Grpc;

internal sealed class NodeGrpcServiceV1 : NodeGrpcService.NodeGrpcServiceBase
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly Node _node;
    private readonly ILogger<NodeGrpcServiceV1> _logger;

    public NodeGrpcServiceV1(
        IHostApplicationLifetime applicationLifetime,
        Node node,
        ILogger<NodeGrpcServiceV1> logger)
    {
        _applicationLifetime = applicationLifetime;
        _node = node;
        _logger = logger;
        _logger.LogDebug("{GrpcServiceType} is created.", nameof(NodeGrpcServiceV1));
    }

    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse());
    }

    public override async Task<StartResponse> Start(StartRequest request, ServerCallContext context)
    {
        _node.SeedEndPoint = EndPointUtility.Parse(request.SeedEndPoint);
        await _node.StartAsync(context.CancellationToken);
        return new StartResponse { NodeInfo = _node.Info };
    }

    public async override Task<StopResponse> Stop(StopRequest request, ServerCallContext context)
    {
        await _node.StopAsync(context.CancellationToken);
        return new StopResponse();
    }

    public override Task<GetInfoResponse> GetInfo(GetInfoRequest request, ServerCallContext context)
    {
        GetInfoResponse Action() => new()
        {
            NodeInfo = _node.Info,
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
            handler => _node.Started += handler,
            handler => _node.Started -= handler,
            () => new GetStartedStreamResponse { NodeInfo = _node.Info });
        await streamer.RunAsync(_applicationLifetime, context.CancellationToken);
    }

    public override async Task GetStoppedStream(
        GetStoppedStreamRequest request,
        IServerStreamWriter<GetStoppedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetStoppedStreamResponse>(
            responseStream,
            handler => _node.Stopped += handler,
            handler => _node.Stopped -= handler);
        await streamer.RunAsync(_applicationLifetime, context.CancellationToken);
    }
}
