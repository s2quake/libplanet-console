#if LIBPLANET_CONSOLE || LIBPLANET_CLIENT
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Grpc;
using static LibplanetConsole.Node.Grpc.NodeGrpcService;

namespace LibplanetConsole.Node.Grpc;

internal sealed class NodeService(GrpcChannel channel)
    : NodeGrpcServiceClient(channel), IDisposable
{
    private ConnectionMonitor<NodeService>? _connection;
    private StreamReceiver<GetStartedStreamResponse>? _startedReceiver;
    private StreamReceiver<GetStoppedStreamResponse>? _stoppedReceiver;
    private bool _isDisposed;

    public event EventHandler? Disconnected;

    public event EventHandler<NodeEventArgs>? Started;

    public event EventHandler? Stopped;

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _startedReceiver?.Dispose();
            _startedReceiver = null;
            _stoppedReceiver?.Dispose();
            _stoppedReceiver = null;
            _connection?.Dispose();
            _connection = null;
            _isDisposed = true;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null)
        {
            throw new InvalidOperationException($"{nameof(NodeService)} is already started.");
        }

        _connection = new ConnectionMonitor<NodeService>(this, CheckConnectionAsync);
        _connection.Disconnected += Connection_Disconnected;
        await _connection.StartAsync(cancellationToken);
        _startedReceiver = new(
            GetStartedStream(new(), cancellationToken: cancellationToken),
            (response) => Started?.Invoke(this, new(response.NodeInfo)));
        _stoppedReceiver = new(
            GetStoppedStream(new(), cancellationToken: cancellationToken),
            (response) => Stopped?.Invoke(this, EventArgs.Empty));
        await Task.WhenAll(
            _startedReceiver.StartAsync(cancellationToken),
            _stoppedReceiver.StartAsync(cancellationToken));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connection is null)
        {
            throw new InvalidOperationException($"{nameof(NodeService)} is not started.");
        }

        if (_startedReceiver is not null)
        {
            await _startedReceiver.StopAsync(cancellationToken);
            _startedReceiver = null;
        }

        if (_stoppedReceiver is not null)
        {
            await _stoppedReceiver.StopAsync(cancellationToken);
            _stoppedReceiver = null;
        }

        _connection.Disconnected -= Connection_Disconnected;
        await _connection.StopAsync(cancellationToken);
        _connection = null;
    }

    private static async Task CheckConnectionAsync(
        NodeService nodeService, CancellationToken cancellationToken)
    {
        await nodeService.PingAsync(new(), cancellationToken: cancellationToken);
    }

    private void Connection_Disconnected(object? sender, EventArgs e)
    {
        if (sender is ConnectionMonitor<NodeService> connection && connection == _connection)
        {
            _startedReceiver?.Dispose();
            _startedReceiver = null;
            _stoppedReceiver?.Dispose();
            _stoppedReceiver = null;
            _connection.Dispose();
            _connection = null;
            Disconnected?.Invoke(this, e);
        }
    }
}
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
