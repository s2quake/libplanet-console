using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class PrivateKeyJsonConverter : JsonConverter<PrivateKey>
{
    public override PrivateKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new JsonException($"{nameof(PrivateKey)} is not supported for security reasons.");
    }

    public override void Write(
        Utf8JsonWriter writer, PrivateKey value, JsonSerializerOptions options)
    {
        throw new JsonException($"{nameof(PrivateKey)} is not supported for security reasons.");
    }
}
