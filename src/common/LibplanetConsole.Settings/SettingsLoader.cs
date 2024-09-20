using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace LibplanetConsole.Settings;

public static class SettingsLoader
{
    public static void Load(string settingsPath, IReadOnlyDictionary<string, object> settingsByName)
    {
        if (Path.IsPathRooted(settingsPath) is false)
        {
            throw new ArgumentException(
                $"'{settingsPath}' must be an absolute path.", nameof(settingsPath));
        }

        var json = File.ReadAllText(settingsPath);
        if (JsonSerializer.Deserialize<Dictionary<string, object>>(json) is not { } obj)
        {
            throw new ArgumentException("Invalid JSON format.", nameof(settingsPath));
        }

        var jso = new JsonSerializerOptions
        {
            PropertyNamingPolicy = SettingsSchemaBuilder.NamingPolicy,
        };
        var directory = Path.GetDirectoryName(settingsPath)
            ?? throw new ArgumentException("Invalid settings path.", nameof(settingsPath));

        foreach (var item in settingsByName)
        {
            var settingsName = item.Key;
            var settings = item.Value;
            var jsonName = SettingsSchemaBuilder.NamingPolicy.ConvertName(settingsName);
            if (obj.TryGetValue(jsonName, out var value) is true)
            {
                Load(settings, value, jso);
                Validate(settings, directory);
            }
        }
    }

    private static void Load(object settings, object props, JsonSerializerOptions jso)
    {
        var json = JsonSerializer.Serialize(props);
        var settingsType = settings.GetType();

        if (JsonSerializer.Deserialize(json, settingsType, jso) is not { } deserializedSettings)
        {
            throw new ArgumentException("Invalid JSON format.", nameof(props));
        }

        foreach (var prop in settings.GetType().GetProperties())
        {
            var value = prop.GetValue(deserializedSettings);
            prop.SetValue(settings, value);
        }
    }

    private static void Validate(object settings, string directory)
    {
        var items = new Dictionary<object, object?>
        {
            ["CurrentDirectory"] = directory,
        };
        var validationContext = new ValidationContext(settings, null, items);
        Validator.ValidateObject(settings, validationContext, true);
    }
}
