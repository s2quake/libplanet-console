using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

internal static class ClientFactory
{
    private static readonly ConcurrentDictionary<IServiceProvider, Descriptor> _valueByKey = [];

    public static Client Create(IServiceProvider serviceProvider)
    {
        if (_valueByKey.Remove(serviceProvider, out var descriptor) is true)
        {
            var clientOptions = descriptor.ClientOptions;
            var client = new Client(serviceProvider, clientOptions);
            client.Disposed += (s, e) => descriptor.ServiceScope.Dispose();
            return client;
        }

        throw new UnreachableException("This should not be called.");
    }

    public static Client CreateNew(IServiceProvider serviceProvider, ClientOptions clientOptions)
    {
        var serviceScope = serviceProvider.CreateScope();
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

        public required IServiceScope ServiceScope { get; init; }
    }
}
