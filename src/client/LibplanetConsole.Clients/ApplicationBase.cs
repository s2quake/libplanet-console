using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net;
using JSSoft.Terminals;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.Clients;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication, INodeCallback
{
    private readonly CompositionContainer _container;
    private readonly ApplicationOptions _options = new();
    private readonly Client _client;
    private readonly IClientContent[] _clientContents;
    private readonly ClientServiceContext _clientServiceContext;
    private readonly Process? _parentProcess;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _options = options.GetActualOptions();
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue<INodeCallback>(this);
        _container.ComposeExportedValue(_options);
        _client = _container.GetExportedValue<Client>() ??
            throw new InvalidOperationException($"'{typeof(Client)}' is not found.");
        _clientContents = _container.GetExportedValues<IClientContent>().ToArray();
        _clientServiceContext = _container.GetExportedValue<ClientServiceContext>() ??
            throw new InvalidOperationException($"'{typeof(ClientServiceContext)}' is not found.");
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        _client.Started += Client_Started;
        _client.Stopped += Client_Stopped;
        if (_options.ParentProcessId != 0 &&
            Process.GetProcessById(_options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _clientServiceContext.EndPoint;

    public ApplicationInfo Info => new()
    {
        ServiceEndPoint = EndPointUtility.ToString(EndPoint),
        ClientInfo = _client.Info,
    };

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object?>(contractName);
    }

    void INodeCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        var hash = blockInfo.Hash[0..8];
        var miner = blockInfo.Miner[0..8];
        var message = $"Block #{blockInfo.Index} '{hash}' Appended by '{miner}'";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightMagenta));
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _closeToken = await _clientServiceContext.StartAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await AutoStartAsync();
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _clientServiceContext.CloseAsync(_closeToken, CancellationToken.None);
        _client.Started -= Client_Started;
        _client.Stopped -= Client_Stopped;
        _container.Dispose();
    }

    private async Task AutoStartAsync()
    {
        if (_options.NodeEndPoint != string.Empty)
        {
            var clientOptions = new ClientOptions
            {
                NodeEndPoint = await _options.GetEndPointAsync(CancellationToken.None),
            };
            await _client.StartAsync(clientOptions, cancellationToken: default);
        }
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        var endPoint = EndPointUtility.ToString(_clientServiceContext.EndPoint);
        var message = $"Client has been started.: {endPoint}";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
        Console.Out.WriteLineAsJson(_client.Info);
    }

    private void Client_Stopped(object? sender, StopEventArgs e)
    {
        var endPoint = EndPointUtility.ToString(_clientServiceContext.EndPoint);
        var message = $"Client has been stopped.: {e.Reason}";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
    }
}
