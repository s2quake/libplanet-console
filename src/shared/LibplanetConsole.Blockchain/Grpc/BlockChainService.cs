#if LIBPLANET_CONSOLE || LIBPLANET_CLIENT
using Grpc.Net.Client;
using LibplanetConsole.Grpc;
using LibplanetConsole.Node;
using LibplanetConsole.Node.Grpc;
using static LibplanetConsole.Blockchain.Grpc.BlockChainGrpcService;

namespace LibplanetConsole.Blockchain.Grpc;

internal sealed class BlockChainService(GrpcChannel channel)
    : BlockChainGrpcServiceClient(channel), IDisposable
{
    private StreamReceiver<GetBlockAppendedStreamResponse>? _blockAppendedReceiver;
    private bool _isDisposed;

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _blockAppendedReceiver?.Dispose();
            _blockAppendedReceiver = null;
            _isDisposed = true;
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_blockAppendedReceiver is not null)
        {
            throw new InvalidOperationException($"{nameof(BlockChainService)} is already started.");
        }

        _blockAppendedReceiver = new(
            GetBlockAppendedStream(new(), cancellationToken: cancellationToken),
            (response) => BlockAppended?.Invoke(this, new(response.BlockInfo)));
        await _blockAppendedReceiver.StartAsync(cancellationToken);
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_blockAppendedReceiver is null)
        {
            throw new InvalidOperationException($"{nameof(BlockChainService)} is not started.");
        }

        await _blockAppendedReceiver.StopAsync(cancellationToken);
        _blockAppendedReceiver = null;
    }
}
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
