using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Alias;
using LibplanetConsole.Alias.Grpc;
using LibplanetConsole.Alias.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Hub.Services;

namespace LibplanetConsole.Node;

internal sealed class AliasCollection(
    IApplicationOptions options, ILogger<AliasCollection> logger)
    : AliasCollectionBase, IAliasCollection, INodeContent
{
    private const string AliasServiceName = "libplanet.console.alias.v1";
    private AliasService? _service;
    private GrpcChannel? _channel;
    private bool _isSingleNode;
    private string _localPath = string.Empty;

    string INodeContent.Name => "aliases";

    IEnumerable<INodeContent> INodeContent.Dependencies { get; } = [];

    async Task INodeContent.StartAsync(CancellationToken cancellationToken)
    {
        if (options.HubUrl is { } hubUrl)
        {
            var aliasUrl = await HubService.GetServiceUrlAsync(
                hubUrl, AliasServiceName, cancellationToken);
            var channel = AliasChannel.CreateChannel(aliasUrl);
            var service = new AliasService(channel);
            var request = new GetAliasesRequest();
            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            var response = await service.GetAliasesAsync(request, callOptions);
            Initialize([.. response.AliasInfos.Select(item => (AliasInfo)item)]);
            await service.InitializeAsync(cancellationToken);

            _channel = channel;
            _service = service;
            _service.AliasAdded += Service_AliasAdded;
            _service.AliasUpdated += Service_AliasUpdated;
            _service.AliasRemoved += Service_AliasRemoved;
        }
        else if (options.IsSingleNode is true)
        {
            _isSingleNode = true;
            _localPath = options.StorePath != string.Empty
                ? Path.Combine(options.StorePath, "aliases.json") : string.Empty;
            Load(_localPath);
        }
    }

    async Task INodeContent.StopAsync(CancellationToken cancellationToken)
    {
        if (_localPath != string.Empty)
        {
            var json = JsonUtility.Serialize(this.ToArray());
            await File.WriteAllTextAsync(_localPath, json, cancellationToken);
        }

        if (_service is not null)
        {
            _service.AliasAdded -= Service_AliasAdded;
            _service.AliasUpdated -= Service_AliasUpdated;
            _service.AliasRemoved -= Service_AliasRemoved;
            await _service.ReleaseAsync(cancellationToken);
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

        _isSingleNode = false;
        _localPath = string.Empty;

        await Task.CompletedTask;
    }

    public async Task AddAsync(AliasInfo aliasInfo, CancellationToken cancellationToken)
    {
        if (_service is not null)
        {
            var request = new AddAliasRequest
            {
                AliasInfo = aliasInfo,
            };
            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            await _service.AddAliasAsync(request, callOptions);
        }
        else if (_isSingleNode is true)
        {
            Add(aliasInfo);
        }
        else
        {
            throw new InvalidOperationException("The alias service is not available.");
        }
    }

    public async Task RemoveAsync(string alias, CancellationToken cancellationToken)
    {
        if (_service is not null)
        {
            var request = new RemoveAliasRequest
            {
                Alias = alias,
            };
            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            await _service.RemoveAliasAsync(request, callOptions);
        }
        else if (_isSingleNode is true)
        {
            Remove(alias);
        }
        else
        {
            throw new InvalidOperationException("The alias service is not available.");
        }
    }

    public async Task UpdateAsync(AliasInfo aliasInfo, CancellationToken cancellationToken)
    {
        if (_service is not null)
        {
            var request = new UpdateAliasRequest
            {
                AliasInfo = aliasInfo,
            };
            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            await _service.UpdateAliasAsync(request, callOptions);
        }
        else if (_isSingleNode is true)
        {
            Update(aliasInfo);
        }
        else
        {
            throw new InvalidOperationException("The alias service is not available.");
        }
    }

    protected override void OnAdded(AliasEventArgs e)
    {
        base.OnAdded(e);
        Save(_localPath);
    }

    protected override void OnRemoved(AliasRemovedEventArgs e)
    {
        base.OnRemoved(e);
        Save(_localPath);
    }

    protected override void OnUpdated(AliasUpdatedEventArgs e)
    {
        base.OnUpdated(e);
        Save(_localPath);
    }

    private new void Save(string path)
    {
        if (path != string.Empty)
        {
            try
            {
                base.Save(path);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to save aliases.");
            }
        }
    }

    private new void Load(string path)
    {
        if (File.Exists(path) is true)
        {
            try
            {
                base.Load(path);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to load aliases.");
            }
        }
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
