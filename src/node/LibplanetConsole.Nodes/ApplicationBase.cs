using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.Nodes;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly Node _node;
    private readonly NodeContext _nodeContext;
    private readonly Process? _parentProcess;
    private readonly bool _isAutoStart;
    private readonly ApplicationInfo _info;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _isAutoStart = options.AutoStart;
        _node = new Node(options);
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_node);
        _container.ComposeExportedValue<INode>(_node);
        _nodeContext = _container.GetExportedValue<NodeContext>() ??
            throw new InvalidOperationException($"'{typeof(NodeContext)}' is not found.");
        _nodeContext.EndPoint = options.EndPoint;
        _info = new()
        {
            EndPoint = EndPointUtility.ToString(_nodeContext.EndPoint),
            NodeEndPoint = $"{options.NodeEndPoint}",
            StorePath = _node.StorePath,
        };
        DefaultGenesisOptions = new GenesisOptions
        {
            GenesisKey = new(),
            GenesisValidators = options.GenesisValidators,
            Timestamp = DateTimeOffset.UtcNow,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _nodeContext.EndPoint;

    public ApplicationInfo Info => _info;

    internal GenesisOptions DefaultGenesisOptions { get; }

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object?>(contractName);
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _closeToken = await _nodeContext.StartAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _nodeContext.CloseAsync(_closeToken, cancellationToken: default);
        _container.Dispose();
    }

    private async Task AutoStartAsync(CancellationToken cancellationToken)
    {
        if (_isAutoStart == true)
        {
            var seedEndPoint = _info.NodeEndPoint;
            var nodeOptions = seedEndPoint != string.Empty
                ? await NodeOptions.CreateAsync(seedEndPoint, cancellationToken)
                : new NodeOptions
                {
                    GenesisOptions = DefaultGenesisOptions,
                };
            await _node.StartAsync(nodeOptions, cancellationToken: default);
        }
    }
}
