using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;

namespace LibplanetConsole.Options;

public class OptionsSchemaBuilder
{
#pragma warning disable S1075 // URIs should not be hardcoded
    public const string BaseSchemaUrl = "https://json.schemastore.org/appsettings.json";
#pragma warning restore S1075 // URIs should not be hardcoded
    private const int Timeout = 10000;

    private readonly Dictionary<string, Type> _typeByName = [];
    private readonly List<string> _requiredNameList = [];

    public static OptionsSchemaBuilder Create()
    {
        var assemblies = GetAssemblies(Assembly.GetEntryAssembly()!);
        var query = from assembly in assemblies
                    from type in assembly.GetTypes()
                    where IsOptionsType(type) == true
                    select type;
        var types = query.Distinct().ToArray();
        var schemaBuilder = new OptionsSchemaBuilder();
        foreach (var type in types)
        {
            var settingsType = type;
            var attributeType = typeof(OptionsAttribute);
            var attribute = Attribute.GetCustomAttribute(settingsType, attributeType);
            if (attribute is not OptionsAttribute settingsAttribute)
            {
                throw new UnreachableException("The attribute is not found.");
            }

            var settingsName = settingsAttribute.GetSettingsName(settingsType);
            schemaBuilder.Add(settingsName, settingsType);
            if (settingsAttribute.IsRequired)
            {
                schemaBuilder.AddRequired(settingsName);
            }
        }

        return schemaBuilder;

        static bool IsOptionsType(Type type)
            => Attribute.IsDefined(type, typeof(OptionsAttribute)) is true;
    }

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
        using var cancellationTokenSource = new CancellationTokenSource(millisecondsDelay: Timeout);
        var task = JsonSchema.FromUrlAsync(BaseSchemaUrl, cancellationTokenSource.Token);
        var originSchema = task.GetAwaiter().GetResult();
        var schema = new JsonSchema();
        var optionsSchema = new JsonSchema
        {
            Type = JsonObjectType.Object,
        };

        schema.Definitions["appsettings"] = originSchema;
        schema.AllOf.Add(new JsonSchema
        {
            Reference = originSchema,
        });
        schema.AllOf.Add(optionsSchema);
        foreach (var (name, type) in _typeByName)
        {
            var settingsName = name;
            var settings = new SystemTextJsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true,
                SerializerOptions = new JsonSerializerOptions
                {
                },
            };
            var schemaGenerator = new OptionsSchemaGenerator(this, settings);
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
            var settingsName = name;
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

    private static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
    {
        var directory = Path.GetDirectoryName(assembly.Location)!;
        var files = Directory.GetFiles(directory, "LibplanetConsole.*.dll");
        string[] paths =
        [
            assembly.Location,
            .. files,
        ];
        return [.. paths.Distinct().Order().Select(Assembly.LoadFrom)];
    }
}
