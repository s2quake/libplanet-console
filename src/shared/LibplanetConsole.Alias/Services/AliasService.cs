#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Net.Client;
using LibplanetConsole.Alias.Grpc;
using LibplanetConsole.Grpc;
using static LibplanetConsole.Alias.Grpc.AliasGrpcService;

namespace LibplanetConsole.Alias.Services;

internal sealed class AliasService(GrpcChannel channel)
    : AliasGrpcServiceClient(channel), IDisposable
{
    private StreamReceiver<GetEventStreamResponse>? _eventReceiver;
    private bool _isDisposed;

    public event EventHandler<AliasEventArgs>? AliasAdded;

    public event EventHandler<AliasUpdatedEventArgs>? AliasUpdated;

    public event EventHandler<AliasRemovedEventArgs>? AliasRemoved;

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
            throw new InvalidOperationException($"{nameof(AliasService)} is already started.");
        }

        _eventReceiver = new(() => GetEventStream(new(), default), InvokeEvent);
        await _eventReceiver.StartAsync(cancellationToken);
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_eventReceiver is null)
        {
            throw new InvalidOperationException($"{nameof(AliasService)} is not started.");
        }

        await _eventReceiver.StopAsync(cancellationToken);
        _eventReceiver = null;
    }

    private void InvokeEvent(GetEventStreamResponse response)
    {
        switch (response.EventCase)
        {
            case GetEventStreamResponse.EventOneofCase.AliasAdded:
                AliasAdded?.Invoke(this, new AliasEventArgs(response.AliasAdded.AliasInfo));
                break;
            case GetEventStreamResponse.EventOneofCase.AliasUpdated:
                AliasUpdated?.Invoke(this, new AliasUpdatedEventArgs(
                    response.AliasUpdated.AliasInfo.Alias, response.AliasUpdated.AliasInfo));
                break;
            case GetEventStreamResponse.EventOneofCase.AliasRemoved:
                AliasRemoved?.Invoke(this, new AliasRemovedEventArgs(response.AliasRemoved.Alias));
                break;
        }
    }
}
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
