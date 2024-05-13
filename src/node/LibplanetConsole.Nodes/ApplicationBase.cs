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
    private readonly ApplicationOptions _options = new();
    private readonly Node _node;
    private readonly NodeContext _nodeContext;
    private readonly INodeContent[] _nodeContents;
    private readonly Process? _parentProcess;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _options = options.GetActualOptions();
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _node = _container.GetExportedValue<Node>() ??
            throw new InvalidOperationException($"'{typeof(Node)}' is not found.");
        _nodeContents = _container.GetExportedValues<INodeContent>().ToArray();
        _nodeContext = _container.GetExportedValue<NodeContext>() ??
            throw new InvalidOperationException($"'{typeof(NodeContext)}' is not found.");
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        if (_options.ParentProcessId != 0 &&
            Process.GetProcessById(_options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _nodeContext.EndPoint;

    public ApplicationInfo Info => new()
    {
        EndPoint = EndPointUtility.ToString(EndPoint),
        NodeInfo = _node.Info,
    };

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
        if (_options.AutoStart == true)
        {
            var seedEndPoint = _options.NodeEndPoint;
            var nodeOptions = seedEndPoint != string.Empty
                ? await NodeOptionsUtility.GetNodeOptionsAsync(seedEndPoint, cancellationToken)
                : NodeOptionsUtility.GetNodeOptions(_node);
            await _node.StartAsync(nodeOptions, cancellationToken: default);
        }
    }
}
