using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

internal static class NodeFactory
{
    private static readonly ConcurrentDictionary<IServiceProvider, Descriptor> _valueByKey = [];
    private static readonly ConcurrentDictionary<Node, AsyncServiceScope> _scopeByNode = [];

    public static Node Create(IServiceProvider serviceProvider)
    {
        if (_valueByKey.Remove(serviceProvider, out var descriptor) is true)
        {
            var nodeOptions = descriptor.NodeOptions;
            var node = new Node(serviceProvider, nodeOptions);
            _scopeByNode.AddOrUpdate(node, descriptor.ServiceScope, (k, v) => v);
            return node;
        }

        throw new UnreachableException("This should not be called.");
    }

    public static async ValueTask DisposeScopeAsync(Node node)
    {
        if (_scopeByNode.Remove(node, out var serviceScope) is true)
        {
            await serviceScope.DisposeAsync();
        }
    }

    public static Node CreateNew(IServiceProvider serviceProvider, NodeOptions nodeOptions)
    {
        var serviceScope = serviceProvider.CreateAsyncScope();
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

        public required AsyncServiceScope ServiceScope { get; init; }
    }
}
