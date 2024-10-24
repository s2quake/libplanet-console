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
    private StreamReceiver<GetEventStreamResponse>? _eventReceiver;
    private bool _isDisposed;

    public event EventHandler? Disconnected;

    public event EventHandler<ClientEventArgs>? Started;

    public event EventHandler? Stopped;

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _eventReceiver?.Dispose();
            _eventReceiver = null;
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
        _eventReceiver = new(
            GetEventStream(new(), cancellationToken: cancellationToken), InvokeEvent);
        await _eventReceiver.StartAsync(cancellationToken);
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_connection is null)
        {
            throw new InvalidOperationException($"{nameof(ClientService)} is not started.");
        }

        if (_eventReceiver is not null)
        {
            await _eventReceiver.StopAsync(cancellationToken);
            _eventReceiver = null;
        }

        _connection.Disconnected -= Connection_Disconnected;
        await _connection.StopAsync(cancellationToken);
        _connection = null;
    }

    public override AsyncUnaryCall<StartResponse> StartAsync(
        StartRequest request, CallOptions options)
    {
        if (_eventReceiver is null)
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
            await _eventReceiver.StopAsync(default);
            try
            {
                return await call.ResponseAsync;
            }
            finally
            {
                await _eventReceiver.StartAsync(default);
            }
        }
    }

    public override AsyncUnaryCall<StopResponse> StopAsync(
        StopRequest request, CallOptions options)
    {
        if (_eventReceiver is null)
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
            await _eventReceiver.StopAsync(default);
            try
            {
                return await call.ResponseAsync;
            }
            finally
            {
                await _eventReceiver.StartAsync(default);
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
            _eventReceiver?.Dispose();
            _eventReceiver = null;
            _connection.Dispose();
            _connection = null;
            Disconnected?.Invoke(this, e);
        }
    }

    private void InvokeEvent(GetEventStreamResponse response)
    {
        switch (response.EventCase)
        {
            case GetEventStreamResponse.EventOneofCase.Started:
                Started?.Invoke(this, new ClientEventArgs(response.Started.ClientInfo));
                break;
            case GetEventStreamResponse.EventOneofCase.Stopped:
                Stopped?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
}
#endif // LIBPLANET_CONSOLE
