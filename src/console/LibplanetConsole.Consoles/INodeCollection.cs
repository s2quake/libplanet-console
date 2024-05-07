using System.Collections.Specialized;
using System.Net;
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

    Task<INode> AddNewAsync(CancellationToken cancellationToken)
        => AddNewAsync(new(), cancellationToken);

    Task<INode> AddNewAsync(PrivateKey privateKey, CancellationToken cancellationToken);

    Task<INode> AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken);
}
