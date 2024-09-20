using System.Text.RegularExpressions;

namespace LibplanetConsole.Frameworks;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ApplicationSettingsAttribute(string name) : Attribute
{
    public ApplicationSettingsAttribute()
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

        return Regex.Replace(type.Name, @"(Settings|Options)$", string.Empty);
    }
}
