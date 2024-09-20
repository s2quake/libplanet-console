using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;

namespace LibplanetConsole.Settings;

public class SettingsSchemaBuilder
{
    internal static readonly JsonNamingPolicy NamingPolicy = JsonNamingPolicy.CamelCase;
    private readonly Dictionary<string, Type> _typeByName = [];
    private readonly List<string> _requiredNameList = [];

    public void Add(string name, Type type)
    {
        _typeByName.Add(name, type);
    }

    public void AddRequired(string name)
    {
        if (name == string.Empty)
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        if (_requiredNameList.Contains(name) is true)
        {
            throw new ArgumentException($"'{name}' is already required.", nameof(name));
        }

        if (_typeByName.ContainsKey(name) is false)
        {
            throw new ArgumentException($"'{name}' is not added.", nameof(name));
        }

        _requiredNameList.Add(name);
    }

    public string Build()
    {
        var schema = new JsonSchema();
        var optionsSchema = new JsonSchema
        {
            Type = JsonObjectType.Object,
        };

        schema.AllOf.Add(optionsSchema);
        foreach (var (name, type) in _typeByName)
        {
            var settingsName = NamingPolicy.ConvertName(name);
            var settings = new SystemTextJsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true,
                SerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = NamingPolicy,
                    DictionaryKeyPolicy = NamingPolicy,
                },
            };
            var schemaGenerator = new SettingsSchemaGenerator(this, settings);
            var typeSchema = schemaGenerator.Generate(type);
            schema.Definitions[settingsName] = typeSchema;
            optionsSchema.Properties.Add(settingsName, new JsonSchemaProperty()
            {
                Description = GetDescription(type),
                Reference = typeSchema,
            });
        }

        foreach (var name in _requiredNameList)
        {
            var settingsName = NamingPolicy.ConvertName(name);
            optionsSchema.RequiredProperties.Add(settingsName);
        }

        return schema.ToJson();
    }

    internal virtual string GetDescription(ContextualType contextualType)
        => GetDescription(contextualType.Context);

    protected virtual string GetDescription(ICustomAttributeProvider customAttributeProvider)
    {
        var attriutes = customAttributeProvider.GetCustomAttributes(
            typeof(DescriptionAttribute), inherit: true);
        var attriute = attriutes.FirstOrDefault();
        if (attriute is DescriptionAttribute descriptionAttribute)
        {
            return descriptionAttribute.Description;
        }

        return $"'{customAttributeProvider}' does not have a description.";
    }
}
