using System.Collections.Specialized;

namespace LibplanetConsole.Console;

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

    Task<IClient> AddNewAsync(AddNewClientOptions newOptions, CancellationToken cancellationToken);

    Task AttachAsync(AttachOptions options, CancellationToken cancellationToken);
}
