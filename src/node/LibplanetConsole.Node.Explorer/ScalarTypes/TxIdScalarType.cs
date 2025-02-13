using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;

namespace LibplanetConsole.Node.Explorer.ScalarTypes;

internal sealed class TxIdScalarType()
    : ScalarGraphTypeBase("TxId", typeof(TxId))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return TxId.FromHex(text);
    }

    public override object Serialize(object item)
        => item is TxId txId ? txId.ToHex() : base.Serialize(item);

    public override bool ValidateObject(object item) => item is TxId;
}
