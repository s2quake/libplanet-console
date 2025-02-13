using System.Globalization;
using System.Numerics;
using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;

namespace LibplanetConsole.Node.Explorer.ScalarTypes;

internal sealed class BigIntegerScalarType()
    : ScalarGraphTypeBase("BigInteger", typeof(BigInteger))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return BigInteger.Parse(text, NumberStyles.Number);
    }

    public override object Serialize(object item)
        => item is BigInteger bigInteger ? bigInteger.ToString("N0") : base.Serialize(item);

    public override bool ValidateObject(object item) => item is BigInteger;
}
