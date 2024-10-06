#pragma warning disable SA1402 // File may only contain a single type
using Grpc.Core;

namespace LibplanetConsole.Node.Services;

internal sealed class EventStreamer<TResponse, TEventArgs>(
    IAsyncStreamWriter<TResponse> streamWriter,
    Action<EventHandler<TEventArgs>> attach,
    Action<EventHandler<TEventArgs>> detach,
    Func<TEventArgs, TResponse> selector) : Streamer<TResponse>(streamWriter)
{
    protected async override Task OnRun(CancellationToken cancellationToken)
    {
        void Handler(object? s, TEventArgs args)
        {
            var value = selector(args);
            WriteValue(value);
        }

        attach(Handler);
        try
        {
            await base.OnRun(cancellationToken);
        }
        finally
        {
            detach(Handler);
        }
    }
}

internal sealed class EventStreamer<TResponse>(
    IAsyncStreamWriter<TResponse> streamWriter,
    Action<EventHandler> attach,
    Action<EventHandler> detach,
    Func<TResponse> selector) : Streamer<TResponse>(streamWriter)
{
    public EventStreamer(
        IAsyncStreamWriter<TResponse> streamWriter,
        Action<EventHandler> attach,
        Action<EventHandler> detach)
        : this(streamWriter, attach, detach, CreateInstanceBinder)
    {
    }

    protected async override Task OnRun(CancellationToken cancellationToken)
    {
        void Handler(object? s, EventArgs args)
        {
            var value = selector();
            WriteValue(value);
        }

        attach(Handler);
        try
        {
            await base.OnRun(cancellationToken);
        }
        finally
        {
            detach(Handler);
        }
    }

    private static TResponse CreateInstanceBinder()
        => Activator.CreateInstance<TResponse>()
            ?? throw new InvalidOperationException(
                "Failed to create an instance of TResponse.");
}
