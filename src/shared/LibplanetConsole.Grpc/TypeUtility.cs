using System.Security.Cryptography;
using Google.Protobuf;

namespace LibplanetConsole.Grpc;

public static class TypeUtility
{
    private static readonly Codec _codec = new();

    public static AddressProto ToGrpc(Address address)
        => new() { Bytes = ByteString.CopyFrom([.. address.ByteArray]) };

    public static BlockHashProto ToGrpc(BlockHash blockHash)
        => new() { Bytes = ByteString.CopyFrom([.. blockHash.ByteArray]) };

    public static TxIdProto ToGrpc(TxId txId)
        => new() { Bytes = ByteString.CopyFrom(txId.ToByteArray()) };

    public static PublicKeyProto ToGrpc(PublicKey publicKey)
        => new() { Bytes = ByteString.CopyFrom(publicKey.Format(compress: false)) };

    public static HashDigest256Proto ToGrpc(HashDigest<SHA256> hashDigest)
        => new() { Bytes = ByteString.CopyFrom(hashDigest.ToByteArray()) };

    public static FungibleAssetValueProto ToGrpc(FungibleAssetValue value)
        => new() { Bytes = ByteString.CopyFrom(_codec.Encode(value.Serialize())) };

    public static CurrencyProto ToGrpc(Currency currency)
        => new() { Bytes = ByteString.CopyFrom(_codec.Encode(currency.Serialize())) };

    public static ByteString ToGrpc(byte[] bytes)
        => ByteString.CopyFrom(bytes);

    public static Address ToAddress(AddressProto address)
        => new(address.Bytes.ToByteArray());

    public static BlockHash ToBlockHash(BlockHashProto blockHash)
        => new(blockHash.Bytes.ToByteArray());

    public static TxId ToTxId(TxIdProto txId)
        => new(txId.Bytes.ToByteArray());

    public static PublicKey ToPublicKey(PublicKeyProto publicKey)
        => new(publicKey.Bytes.ToByteArray());

    public static HashDigest<SHA256> ToHashDigest256(HashDigest256Proto hashDigest)
        => new(hashDigest.Bytes.ToByteArray());

    public static byte[] ToByteArray(ByteString byteString)
        => byteString.ToByteArray();

    public static FungibleAssetValue ToFungibleAssetValue(FungibleAssetValueProto value)
        => new(_codec.Decode(value.Bytes.ToByteArray()));

    public static Currency ToCurrency(CurrencyProto currency)
        => new(_codec.Decode(currency.Bytes.ToByteArray()));
}
