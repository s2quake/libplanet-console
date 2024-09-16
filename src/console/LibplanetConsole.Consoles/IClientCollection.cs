using System.Collections.Specialized;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public interface IClientCollection : IEnumerable<IClient>, INotifyCollectionChanged
{
    event EventHandler? CurrentChanged;

    int Count { get; }

    IClient? Current { get; set; }

    IClient this[int index] { get; }

    IClient this[AppAddress address] { get; }

    bool Contains(IClient item);

    bool Contains(AppAddress address);

    int IndexOf(IClient item);

    int IndexOf(AppAddress address);

    Task<IClient> AddNewAsync(AddNewClientOptions options, CancellationToken cancellationToken);
}
