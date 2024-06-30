using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppPeerJsonConverter : JsonConverter<AppPeer>
{
    public override AppPeer Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppPeer.Parse(text);
        }

        throw new JsonException("Cannot read AppPeer from JSON.");
    }

    public override void Write(Utf8JsonWriter writer, AppPeer value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
