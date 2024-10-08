using Grpc.Net.Client;
using LibplanetConsole.Node;

namespace LibplanetConsole.Client.Grpc;

internal sealed class BlockChainService(GrpcChannel channel)
    : Node.Grpc.BlockChainGrpcService.BlockChainGrpcServiceClient(channel), IDisposable
{
    private StreamReceiver<Node.Grpc.GetBlockAppendedStreamResponse>? _blockAppendedReceiver;
    private bool _isDisposed;

    public event EventHandler<BlockInfo>? BlockAppended;

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _blockAppendedReceiver?.Dispose();
            _blockAppendedReceiver = null;
            _isDisposed = true;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_blockAppendedReceiver is not null)
        {
            throw new InvalidOperationException($"{nameof(BlockChainService)} is already started.");
        }

        _blockAppendedReceiver = new(
            GetBlockAppendedStream(new(), cancellationToken: cancellationToken),
            (response) => BlockAppended?.Invoke(this, response.BlockInfo));
        await _blockAppendedReceiver.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_blockAppendedReceiver is null)
        {
            throw new InvalidOperationException($"{nameof(BlockChainService)} is not started.");
        }

        await _blockAppendedReceiver.StopAsync(cancellationToken);
        _blockAppendedReceiver = null;
    }
}
