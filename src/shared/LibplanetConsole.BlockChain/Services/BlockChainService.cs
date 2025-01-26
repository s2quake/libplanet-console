#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
using Grpc.Net.Client;
using LibplanetConsole.Grpc;
using LibplanetConsole.Grpc.BlockChain;
using static LibplanetConsole.Grpc.BlockChain.BlockChainGrpcService;

#if LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Services;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Services;
#endif

internal sealed class BlockChainService(GrpcChannel channel)
    : BlockChainGrpcServiceClient(channel), IDisposable
{
    private StreamReceiver<GetEventStreamResponse>? _eventReceiver;
    private bool _isDisposed;

    public event EventHandler<BlockEventArgs>? BlockAppended;

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
            throw new InvalidOperationException($"{nameof(BlockChainService)} is already started.");
        }

        _eventReceiver = new(() => GetEventStream(new(), default), InvokeEvent);
        await _eventReceiver.StartAsync(cancellationToken);
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_eventReceiver is null)
        {
            throw new InvalidOperationException($"{nameof(BlockChainService)} is not started.");
        }

        await _eventReceiver.StopAsync(cancellationToken);
        _eventReceiver = null;
    }

    private void InvokeEvent(GetEventStreamResponse response)
    {
        switch (response.EventCase)
        {
            case GetEventStreamResponse.EventOneofCase.BlockAppended:
                BlockAppended?.Invoke(this, new BlockEventArgs(response.BlockAppended.BlockInfo));
                break;
        }
    }
}
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
