using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class PublicKeyJsonConverter : JsonConverter<PublicKey>
{
    public override PublicKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return PublicKey.FromHex(text);
        }

        throw new JsonException("Cannot read PublicKey from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, PublicKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
