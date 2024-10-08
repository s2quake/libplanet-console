using Grpc.Net.Client;
using LibplanetConsole.Node;

namespace LibplanetConsole.Client.Grpc;

internal sealed class NodeService(GrpcChannel channel)
    : Node.Grpc.NodeGrpcService.NodeGrpcServiceClient(channel), IDisposable
{
    private Connection? _connection;
    private StreamReceiver<Node.Grpc.GetStartedStreamResponse>? _startedReceiver;
    private StreamReceiver<Node.Grpc.GetStoppedStreamResponse>? _stoppedReceiver;
    private bool _isDisposed;

    public event EventHandler? Disconnected;

    public event EventHandler<NodeInfo>? Started;

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

        _connection = new Connection(this);
        _connection.Disconnected += Connection_Disconnected;
        await _connection.StartAsync(cancellationToken);
        _startedReceiver = new(
            GetStartedStream(new(), cancellationToken: cancellationToken),
            (response) => Started?.Invoke(this, response.NodeInfo));
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

    private void Connection_Disconnected(object? sender, EventArgs e)
    {
        if (sender is Connection connection && connection == _connection)
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
