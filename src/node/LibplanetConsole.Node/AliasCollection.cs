using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Alias;
using LibplanetConsole.Alias.Grpc;
using LibplanetConsole.Alias.Services;
using LibplanetConsole.Hub.Services;

namespace LibplanetConsole.Node;

internal sealed class AliasCollection(IApplicationOptions options)
    : AliasCollectionBase, INodeContent
{
    private const string AliasServiceName = "libplanet.console.alias.v1";
    private AliasService? _service;
    private GrpcChannel? _channel;

    string INodeContent.Name => "aliases";

    IEnumerable<INodeContent> INodeContent.Dependencies { get; } = [];

    async Task INodeContent.StartAsync(CancellationToken cancellationToken)
    {
        if (options.HubUrl is { } hubUrl)
        {
            var aliasUrl = await HubService.GetServiceUrlAsync(
                hubUrl, AliasServiceName, cancellationToken);
            _channel = AliasChannel.CreateChannel(aliasUrl);
            _service = new AliasService(_channel);
        }

        if (_service is not null)
        {
            var request = new GetAliasesRequest();
            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            var response = await _service.GetAliasesAsync(request, callOptions);
            foreach (var alias in response.AliasInfos)
            {
                Add(alias);
            }

            _service.AliasAdded += Service_AliasAdded;
            _service.AliasUpdated += Service_AliasUpdated;
            _service.AliasRemoved += Service_AliasRemoved;
        }
    }

    async Task INodeContent.StopAsync(CancellationToken cancellationToken)
    {
        if (_service is not null)
        {
            _service.AliasAdded -= Service_AliasAdded;
            _service.AliasUpdated -= Service_AliasUpdated;
            _service.AliasRemoved -= Service_AliasRemoved;
        }

        Clear();

        if (_service is not null)
        {
            _service.Dispose();
            _service = null;
        }

        if (_channel is not null)
        {
            _channel.Dispose();
            _channel = null;
        }

        await Task.CompletedTask;
    }

    private void Service_AliasAdded(object? sender, AliasEventArgs e)
    {
        Add(e.AliasInfo);
    }

    private void Service_AliasUpdated(object? sender, AliasUpdatedEventArgs e)
    {
        Update(e.Alias, e.AliasInfo);
    }

    private void Service_AliasRemoved(object? sender, AliasRemovedEventArgs e)
    {
        Remove(e.Alias);
    }
}
