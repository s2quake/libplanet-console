using System.Collections.Specialized;
using Libplanet.Crypto;

namespace LibplanetConsole.Consoles;

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

    Task<INode> AddNewAsync(AddNewOptions options, CancellationToken cancellationToken);
}
