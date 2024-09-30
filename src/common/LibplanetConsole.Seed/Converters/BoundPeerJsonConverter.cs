using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Seed.Converters;

public sealed class BoundPeerJsonConverter : JsonConverter<BoundPeer>
{
    public override BoundPeer Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return BoundPeerUtility.Parse(text);
        }

        throw new JsonException("Cannot read BoundPeer from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, BoundPeer value, JsonSerializerOptions options)
    {
        var text = BoundPeerUtility.ToString(value);
        writer.WriteStringValue(text);
    }
}
