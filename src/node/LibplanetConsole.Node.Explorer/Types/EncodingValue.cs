#pragma warning disable S101 // Types should be named in PascalCase
using System.Text;
using System.Text.Json;
using GraphQL.AspNet.Attributes;

namespace LibplanetConsole.Node.Explorer.Types;

[GraphType("IValue")]
public sealed class EncodingValue(IValue value)
{
    private static readonly Codec _codec = new();

    public string Hex => ByteUtil.Hex(_codec.Encode(value));

    public string Base64 => Convert.ToBase64String(_codec.Encode(value));

    public string Inspection => value.Inspect();

    public string Json
    {
        get
        {
            var converter = new Bencodex.Json.BencodexJsonConverter();
            var buffer = new MemoryStream();
            var writer = new Utf8JsonWriter(buffer);
            converter.Write(writer, value, new JsonSerializerOptions());
            return Encoding.UTF8.GetString(buffer.ToArray());
        }
    }
}
