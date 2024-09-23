using System.Diagnostics;
using System.Reflection;
using JSSoft.Commands;
using LibplanetConsole.Settings;

namespace LibplanetConsole.Framework;

public sealed class ApplicationSettingsSchemaBuilder : SettingsSchemaBuilder
{
    public ApplicationSettingsSchemaBuilder()
    {
        var settingsCollection = new ApplicationSettingsCollection();
        foreach (var settings in settingsCollection)
        {
            var settingsType = settings.GetType();
            var attributeType = typeof(ApplicationSettingsAttribute);
            var attribute = Attribute.GetCustomAttribute(settingsType, attributeType);
            if (attribute is not ApplicationSettingsAttribute settingsAttribute)
            {
                throw new UnreachableException("The attribute is not found.");
            }

            var settingsName = settingsAttribute.GetSettingsName(settingsType);
            Add(settingsName, settingsType);
            if (settingsAttribute.IsRequired)
            {
                AddRequired(settingsName);
            }
        }
    }

    protected override string GetDescription(ICustomAttributeProvider customAttributeProvider)
    {
        var attriutes = customAttributeProvider.GetCustomAttributes(
            typeof(CommandSummaryAttribute), inherit: true);
        var attriute = attriutes.FirstOrDefault();
        if (attriute is CommandSummaryAttribute summaryAttribute)
        {
            return summaryAttribute.Summary;
        }

        return $"'{customAttributeProvider}' does not have a description.";
    }
}
