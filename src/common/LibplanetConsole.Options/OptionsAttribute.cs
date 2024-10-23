using System.Text.RegularExpressions;

namespace LibplanetConsole.Options;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OptionsAttribute(string name) : Attribute
{
    public OptionsAttribute()
        : this(string.Empty)
    {
    }

    public string Name { get; } = name;

    public bool IsRequired { get; set; }

    public string GetSettingsName(Type type)
    {
        if (Name != string.Empty)
        {
            return Name;
        }

        return Regex.Replace(type.Name, @"(Options)$", string.Empty);
    }
}
