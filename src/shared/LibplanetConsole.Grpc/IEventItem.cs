using Grpc.Core;

namespace LibplanetConsole.Grpc;

internal interface IEventItem<out TResponse>
{
    void Attach(EventHandler handler);

    void Detach(EventHandler handler);

    TResponse GetResponse(EventArgs e);
}
