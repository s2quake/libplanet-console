using Grpc.Core;
using Grpc.Net.Client;
using Libplanet.Types.Evidence;
using LibplanetConsole.Console.Evidence.Services;
using LibplanetConsole.Evidence;
using LibplanetConsole.Grpc.Evidence;
using Microsoft.Extensions.DependencyInjection;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Evidence;

internal sealed class NodeEvidence([FromKeyedServices(INode.Key)] INode node)
    : NodeContentBase("evidence"), INodeEvidence
{
    private GrpcChannel? _channel;
    private EvidenceGrpcService.EvidenceGrpcServiceClient? _client;

    public async Task<EvidenceId> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new AddEvidenceRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _client.AddEvidenceAsync(request, callOptions);
        return ToEvidenceId(response.EvidenceId);
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

    public async Task<EvidenceInfo> GetEvidenceAsync(
        EvidenceId evidenceId, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new GetEvidenceRequest { EvidenceId = ToGrpc(evidenceId) };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _client.GetEvidenceAsync(request, callOptions);
        return (EvidenceInfo)response.EvidenceInfos[0];
    }

    public async Task<EvidenceInfo[]> GetPendingEvidenceAsync(CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new GetPendingEvidenceRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _client.GetPendingEvidenceAsync(request, callOptions);
        return [.. response.EvidenceInfos.Select(item => (EvidenceInfo)item)];
    }

    public async Task<EvidenceInfo> GetPendingEvidenceAsync(
        EvidenceId evidenceId, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new GetPendingEvidenceRequest
        {
            EvidenceId = ToGrpc(evidenceId),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _client.GetPendingEvidenceAsync(request, callOptions);
        return (EvidenceInfo)response.EvidenceInfos.Single();
    }

    public async Task ViolateAsync(string type, CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("The channel is not initialized.");
        }

        var request = new ViolateRequest
        {
            Type = type,
        };
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
}
