using System.Collections.Specialized;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public interface INodeCollection : IEnumerable<INode>, INotifyCollectionChanged
{
    event EventHandler? CurrentChanged;

    int Count { get; }

    INode? Current { get; set; }

    INode this[int index] { get; }

    INode this[AppAddress address] { get; }

    bool Contains(INode item);

    bool Contains(AppAddress address);

    int IndexOf(INode item);

    int IndexOf(AppAddress address);

    Task<INode> AddNewAsync(AddNewNodeOptions options, CancellationToken cancellationToken);
}
