using Grpc.Core;
using Grpc.Net.Client;

namespace LibplanetConsole.Client.Services;

public abstract class GrpcClientContentBase<T>(IClient client, string name)
    : ClientContentBase(name)
    where T : ClientBase
{
    private GrpcChannel? _channel;
    private T? _service;

    protected T Service
        => _service ?? throw new InvalidOperationException($"{Name} service is not available.");

    protected virtual T CreateService(GrpcChannel channel)
    {
        var args = new object[] { channel };
        if (Activator.CreateInstance(typeof(T), args: args) is T service)
        {
            return service;
        }

        throw new InvalidOperationException("Failed to create service.");
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var nodeUrl = client.HubUrl;
        var address = nodeUrl.ToString();
        _channel = GrpcChannel.ForAddress(address);
        _service = CreateService(_channel);

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _channel = null;

        await Task.CompletedTask;
    }
}
