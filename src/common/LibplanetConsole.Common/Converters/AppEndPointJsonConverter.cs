using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class AppEndPointJsonConverter : JsonConverter<AppEndPoint>
{
    public override AppEndPoint? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return AppEndPoint.Parse(text);
        }

        throw new JsonException("Cannot read AppEndPoint from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, AppEndPoint value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
