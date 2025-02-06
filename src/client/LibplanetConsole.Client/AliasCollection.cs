using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Alias;
using LibplanetConsole.Alias.Grpc;
using LibplanetConsole.Alias.Services;
using LibplanetConsole.Hub.Services;

namespace LibplanetConsole.Client;

internal sealed class AliasCollection(IApplicationOptions options, ILogger<AliasCollection> logger)
    : AliasCollectionBase, IAliasCollection, IClientContent
{
    private const string AliasServiceName = "libplanet.console.alias.v1";
    private AliasService? _service;
    private GrpcChannel? _channel;

    string IClientContent.Name => "aliases";

    IEnumerable<IClientContent> IClientContent.Dependencies { get; } = [];

    async Task IClientContent.StartAsync(CancellationToken cancellationToken)
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

    async Task IClientContent.StopAsync(CancellationToken cancellationToken)
    {
        if (_service is not null)
        {
            _service.AliasAdded -= Service_AliasAdded;
            _service.AliasUpdated -= Service_AliasUpdated;
            _service.AliasRemoved -= Service_AliasRemoved;
        }

        Release();

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

    public async Task AddAsync(AliasInfo aliasInfo, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The alias service is not available.");
        }

        var request = new AddAliasRequest
        {
            AliasInfo = aliasInfo,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.AddAliasAsync(request, callOptions);
    }

    public async Task RemoveAsync(string alias, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The alias service is not available.");
        }

        var request = new RemoveAliasRequest
        {
            Alias = alias,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.RemoveAliasAsync(request, callOptions);
    }

    public async Task UpdateAsync(AliasInfo aliasInfo, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The alias service is not available.");
        }

        var request = new UpdateAliasRequest
        {
            AliasInfo = aliasInfo,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.UpdateAliasAsync(request, callOptions);
    }

    private void Service_AliasAdded(object? sender, AliasEventArgs e)
    {
        try
        {
            Add(e.AliasInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add alias.");
        }
    }

    private void Service_AliasUpdated(object? sender, AliasUpdatedEventArgs e)
    {
        try
        {
            Update(e.AliasInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update alias.");
        }
    }

    private void Service_AliasRemoved(object? sender, AliasRemovedEventArgs e)
    {
        Remove(e.Alias);
    }
}
