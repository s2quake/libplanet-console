using Grpc.Core;
using LibplanetConsole.Alias;
using LibplanetConsole.Alias.Grpc;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Console.Services;

internal sealed class AliasGrpcServiceV1(
    AliasCollection aliases, IHostApplicationLifetime applicationLifetime)
    : AliasGrpcService.AliasGrpcServiceBase
{
    public override Task<GetAliasesResponse> GetAliases(
        GetAliasesRequest request, ServerCallContext context)
    {
        var aliasInfos = aliases.Select(item => (AliasInfoProto)item).ToArray();
        return Task.FromResult(new GetAliasesResponse { AliasInfos = { aliasInfos } });
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
                new EventItem<GetEventStreamResponse, AliasEventArgs>
                {
                    Attach = handler => aliases.Added += handler,
                    Detach = handler => aliases.Added -= handler,
                    Selector = (e) => new GetEventStreamResponse
                    {
                        AliasAdded = new()
                        {
                            AliasInfo = e.AliasInfo,
                        },
                    },
                },
                new EventItem<GetEventStreamResponse, AliasUpdatedEventArgs>
                {
                    Attach = handler => aliases.Updated += handler,
                    Detach = handler => aliases.Updated -= handler,
                    Selector = (e) => new GetEventStreamResponse
                    {
                        AliasUpdated = new()
                        {
                            Alias = e.Alias,
                            AliasInfo = e.AliasInfo,
                        },
                    },
                },
                new EventItem<GetEventStreamResponse, AliasRemovedEventArgs>
                {
                    Attach = handler => aliases.Removed += handler,
                    Detach = handler => aliases.Removed -= handler,
                    Selector = (e) => new GetEventStreamResponse
                    {
                        AliasRemoved = new()
                        {
                            Alias = e.Alias,
                        },
                    },
                },
            },
        };

        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }
}
