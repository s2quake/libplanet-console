using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppIdJsonConverter : JsonConverter<AppId>
{
    public override AppId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppId.Parse(text);
        }

        throw new JsonException("Cannot read AppId from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
