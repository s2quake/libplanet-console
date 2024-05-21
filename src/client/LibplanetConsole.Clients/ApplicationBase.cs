using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly Client _client;
    private readonly ClientServiceContext _clientServiceContext;
    private readonly Process? _parentProcess;
    private readonly bool _isSeed;
    private readonly ApplicationInfo _info;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _client = new(this, options.PrivateKey);
        _isSeed = options.IsSeed;
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_client);
        _container.ComposeExportedValue<IClient>(_client);
        _clientServiceContext = _container.GetExportedValue<ClientServiceContext>() ??
            throw new InvalidOperationException($"'{typeof(ClientServiceContext)}' is not found.");
        _clientServiceContext.EndPoint = options.EndPoint;
        _info = new()
        {
            EndPoint = EndPointUtility.ToString(_clientServiceContext.EndPoint),
            NodeEndPoint = options.NodeEndPoint is EndPoint nodeEndPoint ?
                EndPointUtility.ToString(nodeEndPoint) :
                string.Empty,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _clientServiceContext.EndPoint;

    public ApplicationInfo Info => _info;

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object?>(contractName);
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _closeToken = await _clientServiceContext.StartAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _clientServiceContext.CloseAsync(_closeToken, CancellationToken.None);
        _container.Dispose();
    }

    private async Task AutoStartAsync(CancellationToken cancellationToken)
    {
        if (_info.NodeEndPoint != string.Empty)
        {
            var clientOptions = new ClientOptions
            {
                NodeEndPoint = await GetNodeEndPointAsync(cancellationToken),
            };
            await _client.StartAsync(clientOptions, cancellationToken: cancellationToken);
        }

        async Task<EndPoint> GetNodeEndPointAsync(CancellationToken cancellationToken)
        {
            var nodeEndPoint = EndPointUtility.Parse(_info.NodeEndPoint);
            if (_isSeed == true)
            {
                return await SeedUtility.GetNodeEndPointAsync(nodeEndPoint, cancellationToken);
            }

            return nodeEndPoint;
        }
    }
}
