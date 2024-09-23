using System.Collections;
using System.Diagnostics;
using System.Reflection;
using JSSoft.Commands;

namespace LibplanetConsole.Frameworks;

public sealed class ApplicationSettingsCollection : IEnumerable<object>
{
    private readonly List<object> _settingsList;

    public ApplicationSettingsCollection()
    {
        var assemblies = ApplicationContainer.GetAssemblies();
        var query = from assembly in assemblies
                    from type in assembly.GetTypes()
                    where IsApplicationSettings(type) == true
                    select type;
        var types = query.Distinct().ToArray();
        _settingsList = [.. types.Select(Activator.CreateInstance)];
    }

    public IReadOnlyDictionary<string, object> ToDictionary()
    {
        var dictionary = new Dictionary<string, object>(_settingsList.Count);
        foreach (var settings in _settingsList)
        {
            var settingsName = GetName(settings.GetType());
            dictionary.Add(settingsName, settings);
        }

        return dictionary;
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
        return _settingsList.Find(type.IsInstanceOfType) ??
            throw new InvalidOperationException($"Settings of type {type} is not found.");
    }

    public T Peek<T>() => (T)Peek(typeof(T));

    IEnumerator<object> IEnumerable<object>.GetEnumerator() => _settingsList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _settingsList.GetEnumerator();

    private static bool IsApplicationSettings(Type type)
        => Attribute.IsDefined(type, typeof(ApplicationSettingsAttribute)) is true;

    private static string GetName(Type type)
    {
        var attribute = Attribute.GetCustomAttribute(type, typeof(ApplicationSettingsAttribute));
        if (attribute is not ApplicationSettingsAttribute settingsAttribute)
        {
            throw new UnreachableException("The attribute is not found.");
        }

        return settingsAttribute.GetSettingsName(type);
    }
}
