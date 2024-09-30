// using System.Diagnostics.CodeAnalysis;
// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Bencodex;
// using Bencodex.Types;
// using Libplanet.Common;
// using Libplanet.Crypto;
// using LibplanetConsole.Common.Converters;

// namespace LibplanetConsole.Common;

// [JsonConverter(typeof(AppPublicKeyJsonConverter))]
// public readonly partial record struct AppPublicKey : IFormattable
// {
//     public const string RegularExpression = "(?:[0-9a-fA-F]{130}|[0-9a-fA-F]{66})";
//     private static readonly Codec _codec = new();
//     private readonly PublicKey _publicKey;

//     public AppPublicKey(PublicKey publicKey) => _publicKey = publicKey;

//     public Address Address => _publicKey.Address;

//     public static explicit operator AppPublicKey(PublicKey publicKey)
//         => new(publicKey);

//     public static explicit operator PublicKey(AppPublicKey publicKey)
//         => publicKey._publicKey;

//     public static string ToString(AppPublicKey? publicKey)
//         => publicKey?.ToString() ?? string.Empty;

//     public static AppPublicKey Parse(string text) => new(PublicKey.FromHex(text));

//     public static AppPublicKey? ParseOrDefault(string text)
//         => text == string.Empty ? null : Parse(text);

//     public static bool TryParse(string text, [MaybeNullWhen(false)] out AppPublicKey publicKey)
//     {
//         try
//         {
//             publicKey = Parse(text);
//             return true;
//         }
//         catch
//         {
//             publicKey = default;
//             return false;
//         }
//     }

//     public override string ToString() => _publicKey.ToHex(compress: false);

//     public string ToString(string? format, IFormatProvider? formatProvider)
//     {
//         if (format is "S" or "C")
//         {
//             return _publicKey.ToHex(compress: true);
//         }

//         return ToString();
//     }

//     public string ToShortString() => ToString(format: "S", formatProvider: null);

//     public byte[] ToByteArray() => [.. _publicKey.ToImmutableArray(compress: false)];

//     public string Encrypt(object obj)
//     {
//         var json = JsonSerializer.Serialize(obj);
//         var encodings = Encoding.UTF8.GetBytes(json);
//         var encrypted = _publicKey.Encrypt(encodings);
//         return ByteUtil.Hex(encrypted);
//     }

//     public byte[] Encrypt(byte[] bytes) => _publicKey.Encrypt(bytes);

//     public bool Verify(object obj, byte[] signature)
//     {
//         if (obj is IValue value)
//         {
//             return _publicKey.Verify(_codec.Encode(value), signature);
//         }

//         if (obj is IBencodable bencodable)
//         {
//             return _publicKey.Verify(_codec.Encode(bencodable.Bencoded), signature);
//         }

//         var json = JsonSerializer.Serialize(obj);
//         var bytes = Encoding.UTF8.GetBytes(json);
//         return _publicKey.Verify(bytes, signature);
//     }
// }
