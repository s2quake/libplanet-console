using Bencodex.Types;

namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct ActionInfo
{
    private static readonly Text TypeIdKey = "type_id";

    public ActionInfo(IValue value)
    {
        if (value is Dictionary values && values.TryGetValue(TypeIdKey, out var typeId))
        {
            TypeId = $"{typeId}";
        }
    }

    public ActionInfo()
    {
    }

    public string TypeId { get; init; } = string.Empty;
}
