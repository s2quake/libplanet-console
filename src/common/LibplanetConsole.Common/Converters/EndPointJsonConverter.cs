using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibplanetConsole.Common.Converters;

public sealed class EndPointJsonConverter : JsonConverter<EndPoint>
{
    public override EndPoint? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString() is string text)
        {
            return EndPointUtility.Parse(text);
        }

        throw new JsonException("Cannot read EndPoint from JSON.");
    }

    public override void Write(
        Utf8JsonWriter writer, EndPoint value, JsonSerializerOptions options)
    {
        var text = EndPointUtility.ToString(value);
        writer.WriteStringValue(text);
    }
}
