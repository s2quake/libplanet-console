using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppPublicKeyJsonConverter : JsonConverter<AppPublicKey>
{
    public override AppPublicKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppPublicKey.Parse(text);
        }

        throw new JsonException("Cannot read AppPublicKey from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppPublicKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
