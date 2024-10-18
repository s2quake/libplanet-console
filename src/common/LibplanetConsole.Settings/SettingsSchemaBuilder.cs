using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;
using Json.Schema.Generation;

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
        JsonSchemaBuilder schemaBuilder = new JsonSchemaBuilder();


        // schema.AllOf.Add(optionsSchema);
        foreach (var (name, type) in _typeByName)
        {
            var settingsName = NamingPolicy.ConvertName(name);
            // var settings = new SystemTextJsonSchemaGeneratorSettings
            // {
            //     FlattenInheritanceHierarchy = true,
            //     SerializerOptions = new JsonSerializerOptions
            //     {
            //         PropertyNamingPolicy = NamingPolicy,
            //         DictionaryKeyPolicy = NamingPolicy,
            //     },
            // };
            // var schemaGenerator = new SettingsSchemaGenerator(this, settings);
            // var typeSchema = schemaGenerator.Generate(type);
            // schema.Definitions[settingsName] = typeSchema;
            // optionsSchema.Properties.Add(settingsName, new JsonSchemaProperty()
            // {
            //     Description = GetDescription(type),
            //     Reference = typeSchema,
            // });

            var configuration = new SchemaGeneratorConfiguration();
            {

            }

            var schema = new JsonSchemaBuilder().FromType(type).Build();
            schema.Keywords
            schemaBuilder.Definitions((settingsName, schema));
            // schemaBuilder.AllOf(schema);
        }

        foreach (var name in _requiredNameList)
        {
            var settingsName = NamingPolicy.ConvertName(name);
            schemaBuilder.Required(settingsName);
        }

        JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        };
        return JsonSerializer.Serialize(schemaBuilder.Build(), SerializerOptions);
    }

    // internal virtual string GetDescription(ContextualType contextualType)
    //     => GetDescription(contextualType.Context);

    protected virtual string GetDescription(ICustomAttributeProvider customAttributeProvider)
    {
        // var attriutes = customAttributeProvider.GetCustomAttributes(
        //     typeof(DescriptionAttribute), inherit: true);
        // var attriute = attriutes.FirstOrDefault();
        // if (attriute is DescriptionAttribute descriptionAttribute)
        // {
        //     return descriptionAttribute.Description;
        // }

        // return $"'{customAttributeProvider}' does not have a description.";

        return string.Empty;
    }
}
