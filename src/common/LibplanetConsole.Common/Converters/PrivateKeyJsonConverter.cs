using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

internal sealed class PrivateKeyJsonConverter : JsonConverter<PrivateKey>
{
    public override PrivateKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return new PrivateKey(text);
        }

        throw new JsonException("Cannot read PrivateKey from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, PrivateKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(PrivateKeyUtility.ToString(value));
    }
}
