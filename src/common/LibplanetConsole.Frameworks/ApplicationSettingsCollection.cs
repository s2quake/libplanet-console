using System.Collections;
using System.Reflection;
using JSSoft.Commands;

namespace LibplanetConsole.Frameworks;

public sealed class ApplicationSettingsCollection : IEnumerable<object>
{
    private readonly List<object> _optionList;

    public ApplicationSettingsCollection()
    {
        var assemblies = ApplicationContainer.GetAssemblies();
        var query = from assembly in assemblies
                    from type in assembly.GetTypes()
                    where IsApplicationSettings(type) == true
                    select type;
        var types = query.Distinct().ToArray();
        _optionList = [.. types.Select(Activator.CreateInstance)];
    }

    public void Parse(string[] args)
    {
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(this, commandSettings);
        parser.Parse(args);
    }

    public object Peek(Type type)
    {
        return _optionList.Find(type.IsInstanceOfType) ??
            throw new InvalidOperationException($"Settings of type {type} is not found.");
    }

    public T Peek<T>() => (T)Peek(typeof(T));

    IEnumerator<object> IEnumerable<object>.GetEnumerator() => _optionList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _optionList.GetEnumerator();

    private static bool IsApplicationSettings(Type type)
        => Attribute.IsDefined(type, typeof(ApplicationSettingsAttribute)) is true;
}
