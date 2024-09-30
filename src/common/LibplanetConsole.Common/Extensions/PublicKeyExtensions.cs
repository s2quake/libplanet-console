using System.Text;
using System.Text.Json;

namespace LibplanetConsole.Common.Extensions;

public static class PublicKeyExtensions
{
    private static readonly Codec _codec = new();

    public static bool Verify(this PublicKey @this, object obj, byte[] signature)
    {
        if (obj is IValue value)
        {
            return @this.Verify(_codec.Encode(value), signature);
        }

        if (obj is IBencodable bencodable)
        {
            return @this.Verify(_codec.Encode(bencodable.Bencoded), signature);
        }

        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        return @this.Verify(bytes, signature);
    }
}
