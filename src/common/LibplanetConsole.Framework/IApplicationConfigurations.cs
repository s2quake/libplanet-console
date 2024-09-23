namespace LibplanetConsole.Framework;

public interface IApplicationConfigurations : IEnumerable<string>
{
    int Count { get; }

    string this[string key] { get; set; }

    bool Remove(string key);

    bool ContainsKey(string key);
}
