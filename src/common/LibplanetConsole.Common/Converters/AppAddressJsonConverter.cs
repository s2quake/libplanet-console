using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppAddressJsonConverter : JsonConverter<AppAddress>
{
    public override AppAddress Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppAddress.Parse(text);
        }

        throw new JsonException("Cannot read AppAddress from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
