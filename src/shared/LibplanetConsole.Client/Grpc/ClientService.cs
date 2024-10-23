#if LIBPLANET_CONSOLE
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Console;
using static LibplanetConsole.Grpc.Client.ClientGrpcService;

namespace LibplanetConsole.Grpc.Client;

internal sealed class ClientService(GrpcChannel channel)
    : ClientGrpcServiceClient(channel), IDisposable
{
    private ConnectionMonitor<ClientService>? _connection;
    private StreamReceiver<GetStartedStreamResponse>? _startedReceiver;
    private StreamReceiver<GetStoppedStreamResponse>? _stoppedReceiver;
    private bool _isDisposed;

    public event EventHandler? Disconnected;

    public event EventHandler<ClientEventArgs>? Started;

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

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null)
        {
            throw new InvalidOperationException($"{nameof(ClientService)} is already started.");
        }

        _connection = new ConnectionMonitor<ClientService>(this, CheckConnectionAsync);
        _connection.Disconnected += Connection_Disconnected;
        await _connection.StartAsync(cancellationToken);
        _startedReceiver = new(
            GetStartedStream(new(), cancellationToken: cancellationToken),
            (response) => Started?.Invoke(this, new(response.ClientInfo)));
        _stoppedReceiver = new(
            GetStoppedStream(new(), cancellationToken: cancellationToken),
            (response) => Stopped?.Invoke(this, EventArgs.Empty));
        await Task.WhenAll(
            _startedReceiver.StartAsync(cancellationToken),
            _stoppedReceiver.StartAsync(cancellationToken));
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_connection is null)
        {
            throw new InvalidOperationException($"{nameof(ClientService)} is not started.");
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

    public override AsyncUnaryCall<StartResponse> StartAsync(
        StartRequest request, CallOptions options)
    {
        if (_startedReceiver is null)
        {
            throw new InvalidOperationException($"{nameof(ClientService)} is not initialized.");
        }

        var call = base.StartAsync(request, options);
        return new AsyncUnaryCall<StartResponse>(
            responseAsync: ResponseAsync(),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);

        async Task<StartResponse> ResponseAsync()
        {
            await _startedReceiver.StopAsync(default);
            try
            {
                return await call.ResponseAsync;
            }
            finally
            {
                await _startedReceiver.StartAsync(default);
            }
        }
    }

    public override AsyncUnaryCall<StopResponse> StopAsync(
        StopRequest request, CallOptions options)
    {
        if (_stoppedReceiver is null)
        {
            throw new InvalidOperationException($"{nameof(ClientService)} is not initialized.");
        }

        var call = base.StopAsync(request, options);
        return new AsyncUnaryCall<StopResponse>(
            responseAsync: ResponseAsync(),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);

        async Task<StopResponse> ResponseAsync()
        {
            await _stoppedReceiver.StopAsync(default);
            try
            {
                return await call.ResponseAsync;
            }
            finally
            {
                await _stoppedReceiver.StartAsync(default);
            }
        }
    }

    private static async Task CheckConnectionAsync(
        ClientService clientService, CancellationToken cancellationToken)
    {
        await clientService.PingAsync(new(), cancellationToken: cancellationToken);
    }

    private void Connection_Disconnected(object? sender, EventArgs e)
    {
        if (sender is ConnectionMonitor<ClientService> connection && connection == _connection)
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
#endif // LIBPLANET_CONSOLE
