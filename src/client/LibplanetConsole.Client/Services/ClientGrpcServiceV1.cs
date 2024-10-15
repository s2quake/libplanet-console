using Grpc.Core;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc;
using LibplanetConsole.Grpc.Client;
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
        client.NodeEndPoint = EndPointUtility.Parse(request.NodeEndPoint);
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

    public override async Task GetStartedStream(
        GetStartedStreamRequest request,
        IServerStreamWriter<GetStartedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetStartedStreamResponse>(
            responseStream,
            handler => client.Started += handler,
            handler => client.Started -= handler,
            () => new GetStartedStreamResponse { ClientInfo = client.Info });
        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }

    public override async Task GetStoppedStream(
        GetStoppedStreamRequest request,
        IServerStreamWriter<GetStoppedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetStoppedStreamResponse>(
            responseStream,
            handler => client.Stopped += handler,
            handler => client.Stopped -= handler);
        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }
}
