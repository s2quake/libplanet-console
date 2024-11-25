using System.Collections.Specialized;

namespace LibplanetConsole.Console;

public interface INodeCollection : IEnumerable<INode>, INotifyCollectionChanged
{
    event EventHandler? CurrentChanged;

    int Count { get; }

    INode? Current { get; set; }

    INode this[int index] { get; }

    INode this[Address address] { get; }

    bool Contains(INode item);

    bool Contains(Address address);

    int IndexOf(INode item);

    int IndexOf(Address address);

    Task<INode> AddNewAsync(AddNewNodeOptions newOptions, CancellationToken cancellationToken);

    Task AttachAsync(AttachOptions options, CancellationToken cancellationToken);
}
