// using System.Text.Json;
// using System.Text.Json.Serialization;

// namespace LibplanetConsole.Common.Converters;

// public sealed class TxIdJsonConverter : JsonConverter<TxId>
// {
//     public override TxId Read(
//         ref Utf8JsonReader reader,
//         Type typeToConvert,
//         JsonSerializerOptions options)
//     {
//         if (reader.GetString() is string text)
//         {
//             return TxId.Parse(text);
//         }

//         throw new JsonException("Cannot read TxId from JSON.");
//     }

//     public override void Write(
//         Utf8JsonWriter writer, TxId value, JsonSerializerOptions options)
//     {
//         writer.WriteStringValue(value.ToString());
//     }
// }
