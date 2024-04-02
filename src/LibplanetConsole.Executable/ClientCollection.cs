using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Libplanet.Crypto;

namespace LibplanetConsole.Executable;

[Export]
sealed class ClientCollection : ObservableCollection<Client>
{
    private Client _current;

    public ClientCollection()
        : this(ApplicationOptions.DefaultUserCount)
    {
    }

    public ClientCollection(int count)
        : base(CreateList(count))
    {
        _current = this.First();
    }

    [ImportingConstructor]
    public ClientCollection(ApplicationOptions options)
        : this(options.UserCount)
    {
    }

    public Client this[Address address] => this.First(item => item.Address == address)!;

    public Client Current
    {
        get => _current;
        set
        {
            if (Contains(value) == false)
                throw new ArgumentException($"'{value}' is not included in the collection.", nameof(value));

            _current = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    public int IndexOf(Address address)
    {
        for (var i = 0; i < Count; i++)
        {
            if (this[i].Address == address)
                return i;
        }
        return -1;
    }

    public bool Contains(Address address) => this.Any(item => item.Address == address);

    public event EventHandler? CurrentChanged;

    protected override void InsertItem(int index, Client item)
    {
        if (Contains(item) == true)
            throw new ArgumentException($"'{item}' is already included in the collection.", nameof(item));
        if (Contains(item.Address) == true)
            throw new ArgumentException($"'{item.Address}' is already included in the collection.", nameof(item));

        base.InsertItem(index, item);
        item.Identifier = $"c{Count - 1}";
    }

    private static List<Client> CreateList(int count)
    {
        var list = new List<Client>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(new(name: $"Client{i}") { Out = new ConsoleTextWriter() });
        }
        return list;
    }
}
