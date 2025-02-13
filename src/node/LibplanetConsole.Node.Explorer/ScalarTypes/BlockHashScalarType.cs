using GraphQL.AspNet.Common;
using GraphQL.AspNet.Schemas.TypeSystem.Scalars;
using Libplanet.Types.Blocks;

namespace LibplanetConsole.Node.Explorer.ScalarTypes;

internal sealed class BlockHashScalarType()
    : ScalarGraphTypeBase("BlockHash", typeof(BlockHash))
{
    public override ScalarValueType ValueType => ScalarValueType.String;

    public override TypeCollection OtherKnownTypes => TypeCollection.Empty;

    public override object Resolve(ReadOnlySpan<char> data)
    {
        var text = GraphQLStrings.UnescapeAndTrimDelimiters(data);
        return BlockHash.FromString(text);
    }

    public override object Serialize(object item)
        => item is BlockHash blockHash ? blockHash.ToString() : base.Serialize(item);

    public override bool ValidateObject(object item) => item is BlockHash;
}
