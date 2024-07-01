using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppBlockHashJsonConverter : JsonConverter<AppBlockHash>
{
    public override AppBlockHash Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppBlockHash.Parse(text);
        }

        throw new JsonException("Cannot read AppBlockHash from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppBlockHash value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
