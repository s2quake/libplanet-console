using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;
using Libplanet.Crypto;

namespace LibplanetConsole.Node.Explorer.ScalarTypes;

internal sealed class PublicKeyScalarType()
    : ScalarGraphTypeBase("PublicKey", typeof(PublicKey))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return PublicKey.FromHex(text);
    }

    public override object Serialize(object item)
        => item is PublicKey publicKey ? publicKey.ToHex(compress: false) : base.Serialize(item);

    public override bool ValidateObject(object item) => item is PublicKey;
}
