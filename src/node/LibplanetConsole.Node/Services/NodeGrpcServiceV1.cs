using Grpc.Core;
using LibplanetConsole.Grpc;
using LibplanetConsole.Node.Grpc;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Services;

internal sealed class NodeGrpcServiceV1 : NodeGrpcService.NodeGrpcServiceBase, IDisposable
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly Node _node;
    private readonly ILogger<NodeGrpcServiceV1> _logger;
    private bool _isDisposed;

    public NodeGrpcServiceV1(
        IHostApplicationLifetime applicationLifetime, Node node, ILogger<NodeGrpcServiceV1> logger)
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
        _node.SeedUrl = new(request.HubUrl);
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

    public override async Task GetEventStream(
        GetEventStreamRequest request,
        IServerStreamWriter<GetEventStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetEventStreamResponse>(responseStream)
        {
            Items =
            {
                new EventItem<GetEventStreamResponse>
                {
                    Attach = handler => _node.Started += handler,
                    Detach = handler => _node.Started -= handler,
                    Selector = () => new GetEventStreamResponse
                    {
                        Started = new StartedEvent
                        {
                            NodeInfo = _node.Info,
                        },
                    },
                },
                new EventItem<GetEventStreamResponse>
                {
                    Attach = handler => _node.Stopped += handler,
                    Detach = handler => _node.Stopped -= handler,
                    Selector = () => new GetEventStreamResponse
                    {
                        Stopped = new StoppedEvent(),
                    },
                },
            },
        };

        await streamer.RunAsync(_applicationLifetime, context.CancellationToken);
    }

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _logger.LogDebug("{GrpcServiceType} is disposed.", nameof(NodeGrpcServiceV1));
            _isDisposed = true;
        }
    }
}
