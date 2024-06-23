using Newtonsoft.Json;

namespace LibplanetConsole.Common;

public sealed class AppPeerJsonConverter : JsonConverter<AppPeer>
{
    public override AppPeer ReadJson(
        JsonReader reader,
        Type objectType,
        AppPeer existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.ReadAsString() is string text)
        {
            return AppPeer.Parse(text);
        }

        throw new JsonException("Cannot read AppPeer from JSON.");
    }

    public override void WriteJson(JsonWriter writer, AppPeer value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}
