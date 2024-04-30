using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net;
using JSSoft.Commands.Extensions;
using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.ClientHost.Serializations;
using LibplanetConsole.ClientServices;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.ClientHost;

internal sealed partial class Application : ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly ApplicationOptions _options = new();
    private readonly Client _client;
    private readonly ClientServiceContext _clientServiceContext;
    private readonly Process? _parentProcess;
    private SystemTerminal? _terminal;
    private Guid _closeToken;

    public Application(ApplicationOptions options)
    {
        _options = options;
        _client = new Client(options);
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _container.ComposeExportedValue(_client);
        _container.ComposeExportedValue<IClient>(_client);
        _clientServiceContext = _container.GetExportedValue<ClientServiceContext>()!;
        _client.BlockAppended += Client_BlockAppended;
        _client.Started += Client_Started;
        _client.Stopped += Client_Stopped;
        if (_options.ParentProcessId != 0 &&
            Process.GetProcessById(_options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }
    }

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

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _closeToken = await _clientServiceContext.OpenAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);

        if (_options.ParentProcessId == 0)
        {
            var separator = new string('=', 80);
            var sw = new StringWriter();
            var commandContext = _container.GetExportedValue<CommandContext>()!;
            commandContext.Out = sw;
            await AutoStartAsync();
            sw.WriteLine(TerminalStringBuilder.GetString(separator, TerminalColorType.BrightGreen));
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteLine(TerminalStringBuilder.GetString(separator, TerminalColorType.BrightGreen));
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());

            _terminal = _container.GetExportedValue<SystemTerminal>()!;
            await _terminal!.StartAsync(cancellationToken);
        }
        else
        {
            await AutoStartAsync();
        }
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _clientServiceContext.ReleaseAsync(_closeToken);
        _client.BlockAppended -= Client_BlockAppended;
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

    private void Client_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash[0..8];
        var miner = blockInfo.Miner[0..8];
        var message = $"Block #{blockInfo.Index} '{hash}' Appended by '{miner}'";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightMagenta));
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
