using System.Collections.Concurrent;
using System.Diagnostics;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal static class ClientFactory
{
    private static readonly ConcurrentDictionary<IServiceProvider, Descriptor> _valueByKey = [];
    private static readonly ConcurrentDictionary<Client, AsyncServiceScope> _scopeByClient = [];

    public static Client Create(IServiceProvider serviceProvider, object? key)
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
            await client.DisposeAsync();
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

        var scopedServiceProvider = serviceScope.ServiceProvider;
        var key = IClient.Key;
        var client = scopedServiceProvider.GetRequiredKeyedService<Client>(key);
        client.Contents = GetClientContents(scopedServiceProvider, key);
        return client;
    }

    private static IClientContent[] GetClientContents(IServiceProvider serviceProvider, string key)
    {
        var contents = serviceProvider.GetKeyedServices<IClientContent>(key)
            .OrderBy(item => item.Order);
        return [.. DependencyUtility.TopologicalSort(contents, content => content.Dependencies)];
    }

    private sealed record class Descriptor
    {
        public required ClientOptions ClientOptions { get; init; }

        public required AsyncServiceScope ServiceScope { get; init; }
    }
}
