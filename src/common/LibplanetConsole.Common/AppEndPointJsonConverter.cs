using Newtonsoft.Json;

namespace LibplanetConsole.Common;

public sealed class AppEndPointJsonConverter : JsonConverter<AppEndPoint>
{
    public override AppEndPoint? ReadJson(
        JsonReader reader,
        Type objectType,
        AppEndPoint? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.ReadAsString() is string text)
        {
            return AppEndPoint.Parse(text);
        }

        throw new JsonException("Cannot read AppEndPoint from JSON.");
    }

    public override void WriteJson(JsonWriter writer, AppEndPoint? value, JsonSerializer serializer)
    {
        if (value is not null)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
