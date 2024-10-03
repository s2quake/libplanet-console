using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

internal static class NodeFactory
{
    private static readonly ConcurrentDictionary<IServiceProvider, Descriptor> _valueByKey = [];

    public static Node Create(IServiceProvider serviceProvider)
    {
        if (_valueByKey.Remove(serviceProvider, out var descriptor) is true)
        {
            var nodeOptions = descriptor.NodeOptions;
            var node = new Node(serviceProvider, nodeOptions);
            node.Disposed += (s, e) => descriptor.ServiceScope.Dispose();
            return node;
        }

        throw new UnreachableException("This should not be called.");
    }

    public static Node CreateNew(IServiceProvider serviceProvider, NodeOptions nodeOptions)
    {
        var serviceScope = serviceProvider.CreateScope();
        _valueByKey.AddOrUpdate(
            serviceScope.ServiceProvider,
            new Descriptor
            {
                NodeOptions = nodeOptions,
                ServiceScope = serviceScope,
            },
            (k, v) => v);

        var node = serviceScope.ServiceProvider.GetRequiredService<Node>();
        serviceScope.ServiceProvider.GetServices<INodeContent>();
        return node;
    }

    private sealed record class Descriptor
    {
        public required NodeOptions NodeOptions { get; init; }

        public required IServiceScope ServiceScope { get; init; }
    }
}
