using Grpc.Core;
using LibplanetConsole.Client.Grpc;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client.Services;

internal sealed class ClientGrpcServiceV1(
    IHostApplicationLifetime applicationLifetime, IClient client)
    : ClientGrpcService.ClientGrpcServiceBase
{
    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse());
    }

    public override async Task<StartResponse> Start(StartRequest request, ServerCallContext context)
    {
        client.HubUrl = new Uri(request.HubUrl);
        await client.StartAsync(context.CancellationToken);
        return new StartResponse { ClientInfo = client.Info };
    }

    public async override Task<StopResponse> Stop(StopRequest request, ServerCallContext context)
    {
        await client.StopAsync(context.CancellationToken);
        return new StopResponse();
    }

    public override Task<GetInfoResponse> GetInfo(GetInfoRequest request, ServerCallContext context)
    {
        GetInfoResponse Action() => new()
        {
            ClientInfo = client.Info,
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
                    Attach = handler => client.Started += handler,
                    Detach = handler => client.Started -= handler,
                    Selector = () => new GetEventStreamResponse
                    {
                        Started = new StartedEvent
                        {
                            ClientInfo = client.Info,
                        },
                    },
                },
                new EventItem<GetEventStreamResponse>
                {
                    Attach = handler => client.Stopped += handler,
                    Detach = handler => client.Stopped -= handler,
                    Selector = () => new GetEventStreamResponse
                    {
                        Stopped = new StoppedEvent(),
                    },
                },
            },
        };

        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }
}
