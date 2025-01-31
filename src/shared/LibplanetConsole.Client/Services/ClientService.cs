#if LIBPLANET_CONSOLE
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Client.Grpc;
using LibplanetConsole.Console;
using LibplanetConsole.Grpc;
using static LibplanetConsole.Client.Grpc.ClientGrpcService;

namespace LibplanetConsole.Client.Services;

internal sealed class ClientService : ClientGrpcServiceClient, IDisposable
{
    private StreamReceiver<GetEventStreamResponse>? _eventReceiver;
    private bool _isDisposed;

    private ClientService(GrpcChannel channel)
        : base(channel)
    {
    }

    public event EventHandler? Disconnected;

    public event EventHandler<ClientEventArgs>? Started;

    public event EventHandler? Stopped;

    public ClientInfo Info { get; private set; }

    public static async Task<ClientService> CreateAsync(
        GrpcChannel channel, CancellationToken cancellationToken)
    {
        var service = new ClientService(channel);
        try
        {
            await service.InitializeAsync(cancellationToken);
            return service;
        }
        catch
        {
            service.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _eventReceiver?.Dispose();
            _eventReceiver = null;
            _isDisposed = true;
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_eventReceiver is not null)
        {
            throw new InvalidOperationException($"{nameof(ClientService)} is already started.");
        }

        await PingAsync(new(), cancellationToken: cancellationToken);
        _eventReceiver = new(() => GetEventStream(new(), default), InvokeEvent)
        {
            Name = "ClientService",
        };
        await _eventReceiver.StartAsync(cancellationToken);
        _eventReceiver.Aborted += EventReceiver_Aborted;
        _eventReceiver.Completed += EventReceiver_Completed;
        Info = (await GetInfoAsync(new(), cancellationToken: cancellationToken)).ClientInfo;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_eventReceiver is not null)
        {
            _eventReceiver.Aborted -= EventReceiver_Aborted;
            _eventReceiver.Completed -= EventReceiver_Completed;
            await _eventReceiver.StopAsync(cancellationToken);
            _eventReceiver = null;
        }
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
            _eventReceiver.Aborted -= EventReceiver_Aborted;
            _eventReceiver.Completed -= EventReceiver_Completed;
            await _eventReceiver.StopAsync(default);
            try
            {
                return await call.ResponseAsync;
            }
            finally
            {
                await _eventReceiver.StartAsync(default);
                _eventReceiver.Aborted += EventReceiver_Aborted;
                _eventReceiver.Completed += EventReceiver_Completed;
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
            _eventReceiver.Aborted -= EventReceiver_Aborted;
            _eventReceiver.Completed -= EventReceiver_Completed;
            await _eventReceiver.StopAsync(default);
            try
            {
                return await call.ResponseAsync;
            }
            finally
            {
                await _eventReceiver.StartAsync(default);
                _eventReceiver.Aborted += EventReceiver_Aborted;
                _eventReceiver.Completed += EventReceiver_Completed;
            }
        }
    }

    private async void EventReceiver_Completed(object? sender, EventArgs e)
    {
        await Task.Run(() =>
        {
            _eventReceiver?.Dispose();
            _eventReceiver = null;
            Disconnected?.Invoke(this, e);
        });
    }

    private async void EventReceiver_Aborted(object? sender, EventArgs e)
    {
        await Task.Run(() =>
        {
            _eventReceiver?.Dispose();
            _eventReceiver = null;
            Disconnected?.Invoke(this, e);
        });
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
