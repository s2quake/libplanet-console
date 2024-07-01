using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppHashJsonConverter : JsonConverter<AppHash>
{
    public override AppHash Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppHash.Parse(text);
        }

        throw new JsonException("Cannot read AppHash from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppHash value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
