using System.Reflection;
using JSSoft.Commands;

namespace LibplanetConsole.Frameworks;

public sealed class ApplicationSettingsParser : ICustomCommandDescriptor
{
    private static ApplicationSettingsParser _instance = new();
    private readonly List<object> _optionList;
    private readonly Dictionary<CommandMemberDescriptor, object> _descriptorByInstance;

    private readonly CommandMemberDescriptorCollection _descriptors;

    private ApplicationSettingsParser()
    {
        var assemblies = ApplicationContainer.GetAssemblies();
        var query = from assembly in assemblies
                    from type in assembly.GetTypes()
                    where IsApplicationSettings(type) == true
                    select type;
        var types = query.Distinct().ToArray();
        _optionList = [.. types.Select(Activator.CreateInstance)];
        _descriptorByInstance = GetDescriptors(_optionList);
        _descriptors = new(GetType(), [.. _descriptorByInstance.Keys]);
    }

    public static ApplicationSettingsParser Instance => _instance;

    CommandMemberDescriptorCollection ICustomCommandDescriptor.Members => _descriptors;

    public static void Parse(string[] args)
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("Settings parser is already initialized.");
        }

        var options = _instance = new ApplicationSettingsParser();
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, commandSettings);
        parser.Parse(args);
    }

    public static T Parse<T>(string[] args)
    {
        Parse(args);
        return (T)Pop(typeof(T));
    }

    public static object Peek(Type type)
    {
        if (_instance is null)
        {
            throw new InvalidOperationException("Settings parser is not initialized.");
        }

        return _instance._optionList.Find(type.IsInstanceOfType) ??
            throw new InvalidOperationException($"Settings of type {type} is not found.");
    }

    public static T Peek<T>() => (T)Peek(typeof(T));

    public static object Pop(Type type)
    {
        if (_instance is null)
        {
            throw new InvalidOperationException("Settings parser is not initialized.");
        }

        if (_instance._optionList.Find(type.IsInstanceOfType) is { } option)
        {
            _instance._optionList.Remove(option);
            foreach (var item in _instance._descriptorByInstance.ToArray())
            {
                if (item.Value == option)
                {
                    _instance._descriptorByInstance.Remove(item.Key);
                }
            }

            return option;
        }

        throw new InvalidOperationException($"Settings of type {type} is not found.");
    }

    public static T Pop<T>() => (T)Pop(typeof(T));

    object ICustomCommandDescriptor.GetMemberOwner(CommandMemberDescriptor memberDescriptor)
        => _descriptorByInstance[memberDescriptor];

    private static bool IsApplicationSettings(Type type)
    {
        return Attribute.GetCustomAttribute(type, typeof(ApplicationSettingsAttribute)) is not null;
    }

    private static Dictionary<CommandMemberDescriptor, object> GetDescriptors(
        IReadOnlyList<object> optionList)
    {
        var itemList = new List<KeyValuePair<CommandMemberDescriptor, object>>();
        for (var i = 0; i < optionList.Count; i++)
        {
            var option = optionList[i];
            var descriptors = CommandDescriptor.GetMemberDescriptors(option)
                                .Select(item => KeyValuePair.Create(item, option));
            itemList.AddRange(descriptors);
        }

        return new(itemList);
    }
}
