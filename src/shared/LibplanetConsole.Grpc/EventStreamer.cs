using Grpc.Core;

namespace LibplanetConsole.Grpc;

internal sealed class EventStreamer<TResponse>(IAsyncStreamWriter<TResponse> streamWriter)
    : Streamer<TResponse>(streamWriter)
{
    public IList<IEventItem<TResponse>> Items { get; } = [];

    protected async override Task OnRun(CancellationToken cancellationToken)
    {
        var handlers = new EventHandler[Items.Count];
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];

            var handler = new EventHandler(Handler);
            item.Attach(Handler);
            handlers[i] = handler;

            async void Handler(object? s, EventArgs args)
            {
                var value = item.GetResponse(args);
                await WriteValueAsync(value);
            }
        }

        try
        {
            await base.OnRun(cancellationToken);
        }
        finally
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var handler = handlers[i];
                item.Detach(handler);
            }
        }
    }
}
