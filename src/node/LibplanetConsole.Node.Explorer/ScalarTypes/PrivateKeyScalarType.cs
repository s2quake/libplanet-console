using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;
using Libplanet.Crypto;

namespace LibplanetConsole.Node.Explorer.ScalarTypes;

internal sealed class PrivateKeyScalarType()
    : ScalarGraphTypeBase("PrivateKey", typeof(PrivateKey))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return new PrivateKey(text);
    }

    public override object Serialize(object item)
    {
        if (item is PrivateKey)
        {
            throw new InvalidOperationException("PrivateKey should not be serialized.");
        }

        return base.Serialize(item);
    }

    public override bool ValidateObject(object item) => item is PrivateKey;
}
