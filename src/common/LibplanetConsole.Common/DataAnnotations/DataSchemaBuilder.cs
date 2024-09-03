using System.ComponentModel;
using System.Reflection;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;

namespace LibplanetConsole.Common.DataAnnotations;

public class DataSchemaBuilder
{
    private readonly Dictionary<string, Type> _typeByName = [];

    public void Add(string name, Type type)
    {
        _typeByName.Add(name, type);
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
            var settings = new SystemTextJsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true,
            };
            var schemaGenerator = new DataSchemaGenerator(this, settings);
            var typeSchema = schemaGenerator.Generate(type);
            schema.Definitions[name] = typeSchema;
            optionsSchema.Properties.Add(name, new JsonSchemaProperty()
            {
                Description = GetDescription(type),
                Reference = typeSchema,
            });
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
