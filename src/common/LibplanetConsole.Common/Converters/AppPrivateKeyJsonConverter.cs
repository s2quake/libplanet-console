using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppPrivateKeyJsonConverter : JsonConverter<AppPrivateKey>
{
    public override AppPrivateKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new JsonException($"{nameof(AppPrivateKey)} is not supported for security reasons.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppPrivateKey value, JsonSerializerOptions options)
    {
        throw new JsonException($"{nameof(AppPrivateKey)} is not supported for security reasons.");
    }
}
