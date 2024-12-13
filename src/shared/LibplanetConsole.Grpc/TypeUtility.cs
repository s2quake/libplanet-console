using System.Security.Cryptography;
using Google.Protobuf;

namespace LibplanetConsole.Grpc;

public static class TypeUtility
{
    private static readonly Codec _codec = new();

    public static string ToGrpc(Address address) => address.ToHex();

    public static string ToGrpc(BlockHash blockHash) => blockHash.ToString();

    public static string ToGrpc(TxId txId) => txId.ToHex();

    public static string ToGrpc(PublicKey publicKey) => publicKey.ToHex(compress: false);

    public static string ToGrpc(HashDigest<SHA256> hashDigest) => hashDigest.ToString();

    public static ByteString ToGrpc(byte[] bytes) => ByteString.CopyFrom(bytes);

    public static ByteString ToGrpc(IValue value) => ToGrpc(_codec.Encode(value));

    public static Address ToAddress(string address) => new(address);

    public static BlockHash ToBlockHash(string blockHash) => BlockHash.FromString(blockHash);

    public static TxId ToTxId(string txId) => TxId.FromHex(txId);

    public static PublicKey ToPublicKey(string publicKey) => PublicKey.FromHex(publicKey);

    public static HashDigest<SHA256> ToHashDigest256(string hashDigest)
        => HashDigest<SHA256>.FromString(hashDigest);

    public static byte[] ToByteArray(ByteString byteString) => byteString.ToByteArray();

    public static IValue ToIValue(ByteString byteString) => _codec.Decode(ToByteArray(byteString));
}
