using System.Security.Cryptography;
using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;

namespace LibplanetConsole.Node.Explorer.ScalarTypes;

internal sealed class HashDigestSha256ScalarType()
    : ScalarGraphTypeBase("HashDigestSHA256", typeof(HashDigest<SHA256>))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return HashDigest<SHA256>.FromString(text);
    }

    public override object Serialize(object item)
        => item is HashDigest<SHA256> hash ? hash.ToString() : base.Serialize(item);

    public override bool ValidateObject(object item) => item is HashDigest<SHA256>;
}
