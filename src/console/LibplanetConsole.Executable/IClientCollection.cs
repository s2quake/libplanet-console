using System.Collections.Specialized;
using System.Net;
using Libplanet.Crypto;

namespace LibplanetConsole.Executable;

public interface IClientCollection : IEnumerable<IClient>, INotifyCollectionChanged
{
    event EventHandler? CurrentChanged;

    int Count { get; }

    IClient? Current { get; set; }

    IClient this[int index] { get; }

    IClient this[Address address] { get; }

    bool Contains(IClient item);

    bool Contains(Address address);

    int IndexOf(IClient item);

    int IndexOf(Address address);

    Task<IClient> AddNewAsync(CancellationToken cancellationToken)
        => AddNewAsync(new(), cancellationToken);

    Task<IClient> AddNewAsync(PrivateKey privateKey, CancellationToken cancellationToken);

    Task<IClient> AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken);
}
