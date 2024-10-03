using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

internal static class ClientFactory
{
    private static readonly ConcurrentDictionary<IServiceProvider, Descriptor> _valueByKey = [];
    private static readonly ConcurrentDictionary<Client, AsyncServiceScope> _scopeByClient = [];

    public static Client Create(IServiceProvider serviceProvider)
    {
        if (_valueByKey.Remove(serviceProvider, out var descriptor) is true)
        {
            var clientOptions = descriptor.ClientOptions;
            var client = new Client(serviceProvider, clientOptions);
            _scopeByClient.AddOrUpdate(client, descriptor.ServiceScope, (k, v) => v);
            return client;
        }

        throw new UnreachableException("This should not be called.");
    }

    public static async ValueTask DisposeScopeAsync(Client client)
    {
        if (_scopeByClient.Remove(client, out var serviceScope) is true)
        {
            await serviceScope.DisposeAsync();
        }
    }

    public static Client CreateNew(IServiceProvider serviceProvider, ClientOptions clientOptions)
    {
        var serviceScope = serviceProvider.CreateAsyncScope();
        _valueByKey.AddOrUpdate(
            serviceScope.ServiceProvider,
            new Descriptor
            {
                ClientOptions = clientOptions,
                ServiceScope = serviceScope,
            },
            (k, v) => v);

        var client = serviceScope.ServiceProvider.GetRequiredService<Client>();
        serviceScope.ServiceProvider.GetServices<IClientContent>();
        return client;
    }

    private sealed record class Descriptor
    {
        public required ClientOptions ClientOptions { get; init; }

        public required AsyncServiceScope ServiceScope { get; init; }
    }
}
