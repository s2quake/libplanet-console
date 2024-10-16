using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Services;
using LibplanetConsole.Grpc.Evidence;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Evidence;

internal sealed class Evidence([FromKeyedServices(INode.Key)] INode node)
    : NodeContentBase("evidence"), IEvidence
{
    private GrpcChannel? _channel;
    private EvidenceGrpcService.EvidenceGrpcServiceClient? _client;

    public async Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new AddEvidenceRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _client.AddEvidenceAsync(request, callOptions);
        return response.EvidenceInfo;
    }

    public async Task<EvidenceInfo[]> GetEvidenceAsync(
        long height, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new GetEvidenceRequest { Height = height };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _client.GetEvidenceAsync(request, callOptions);
        return [.. response.EvidenceInfos.Select(item => (EvidenceInfo)item)];
    }

    public async Task ViolateAsync(CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new ViolateRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _client.ViolateAsync(request, callOptions);
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _channel = EvidenceChannel.CreateChannel(node.EndPoint);
        _client = new EvidenceGrpcService.EvidenceGrpcServiceClient(_channel);
        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _channel = null;
        await Task.CompletedTask;
    }

#if LIBPLANET_DPOS
    public Task UnjailAsync(CancellationToken cancellationToken)
    {
        var signature = node.Sign(true);
        return Service.UnjailAsync(signature, cancellationToken);
    }
#endif // LIBPLANET_DPOS
}
