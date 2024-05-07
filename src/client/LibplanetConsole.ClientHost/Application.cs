using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net;
using JSSoft.Commands.Extensions;
using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.ClientHost.Serializations;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;
using ClientContext = LibplanetConsole.ClientHost.Services.ClientContext;

namespace LibplanetConsole.ClientHost;

internal sealed partial class Application : ApplicationBase, IApplication, INodeCallback
{
    private readonly CompositionContainer _container;
    private readonly ApplicationOptions _options = new();
    private readonly Client _client;
    private readonly IClientContent[] _clientContents;
    private readonly ClientContext _clientContext;
    private readonly Process? _parentProcess;
    private SystemTerminal? _terminal;
    private Guid _closeToken;

    public Application(ApplicationOptions options)
    {
        _options = options;
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue<INodeCallback>(this);
        _container.ComposeExportedValue(_options);
        _client = _container.GetExportedValue<Client>() ??
            throw new InvalidOperationException($"'{typeof(Client)}' is not found.");
        _clientContents = _container.GetExportedValues<IClientContent>().ToArray();
        _clientContext = _container.GetExportedValue<ClientContext>() ??
            throw new InvalidOperationException($"'{typeof(ClientContext)}' is not found.");
        _client.Started += Client_Started;
        _client.Stopped += Client_Stopped;
        if (_options.ParentProcessId != 0 &&
            Process.GetProcessById(_options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }
    }

    public EndPoint EndPoint => _clientContext.EndPoint;

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
        if (_options.ParentProcessId == 0)
        {
            var sw = new StringWriter();
            var commandContext = _container.GetExportedValue<CommandContext>()!;
            commandContext.Out = sw;
            _terminal = _container.GetExportedValue<SystemTerminal>()!;
            _closeToken = await _clientContext.OpenAsync(cancellationToken: default);
            await base.OnStartAsync(cancellationToken);
            await AutoStartAsync();
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());

            await _terminal!.StartAsync(cancellationToken);
        }
        else
        {
            _closeToken = await _clientContext.OpenAsync(cancellationToken: default);
            await base.OnStartAsync(cancellationToken);
            await AutoStartAsync();
        }
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _clientContext.ReleaseAsync(_closeToken);
        _client.Started -= Client_Started;
        _client.Stopped -= Client_Stopped;
        _container.Dispose();
    }

    private async Task AutoStartAsync()
    {
        if (_options.SeedEndPoint != string.Empty)
        {
            var seedEndPoint = _options.SeedEndPoint;
            var nodeEndPoint = await SeedUtility.GetNodeEndPointAsync(
                seedEndPoint, cancellationToken: default);
            var clientOptions = new ClientOptions
            {
                NodeEndPoint = nodeEndPoint,
            };
            await _client.StartAsync(clientOptions, cancellationToken: default);
        }
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        var endPoint = EndPointUtility.ToString(_clientContext.EndPoint);
        var message = $"Client has been started.: {endPoint}";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
        Console.Out.WriteLineAsJson(_client.Info);
    }

    private void Client_Stopped(object? sender, StopEventArgs e)
    {
        var endPoint = EndPointUtility.ToString(_clientContext.EndPoint);
        var message = $"Client has been stopped.: {e.Reason}";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
    }
}
