using System.ComponentModel.DataAnnotations;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;

namespace LibplanetConsole.Common.DataAnnotations;

internal sealed class SetttingsSchemaGenerator(
    SettingsSchemaBuilder schemaBuilder, JsonSchemaGeneratorSettings settings)
    : JsonSchemaGenerator(settings)
{
    public override void ApplyDataAnnotations(
        JsonSchema schema, JsonTypeDescription typeDescription)
    {
        base.ApplyDataAnnotations(schema, typeDescription);

        var contextualType = typeDescription.ContextualType;
        if (typeDescription.ContextualType.OriginalType.IsArray && schema.Item is not null)
        {
            var arrayAttribute = contextualType.GetContextAttributes(true)
                .OfType<ArrayAttribute>()
                .FirstOrDefault();
            if (arrayAttribute != null)
            {
                var validationAttribute = arrayAttribute.CreateAttribute();
                if (validationAttribute is RegularExpressionAttribute regexAttribute)
                {
                    schema.Item.Pattern = regexAttribute.Pattern;
                }
            }
        }

        schema.Description ??= schemaBuilder.GetDescription(contextualType);
    }

    protected override void GenerateEnum(JsonSchema schema, JsonTypeDescription typeDescription)
    {
        var contextualType = typeDescription.ContextualType;

        schema.Type = JsonObjectType.String;
        schema.Enumeration.Clear();
        schema.EnumerationNames.Clear();
        schema.IsFlagEnumerable = contextualType.IsAttributeDefined<FlagsAttribute>(true);

        foreach (var enumName in Enum.GetNames(contextualType.Type))
        {
            schema.Enumeration.Add(enumName);
        }

        if (Settings.GenerateEnumMappingDescription)
        {
            schema.Description = (schema.Description + "\n\n" +
                string.Join("\n", schema.EnumerationNames)).Trim();
        }
    }
}
