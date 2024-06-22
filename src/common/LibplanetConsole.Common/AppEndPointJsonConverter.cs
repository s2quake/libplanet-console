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
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, AppEndPoint? value, JsonSerializer serializer)
    {
        if (value is not null)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
