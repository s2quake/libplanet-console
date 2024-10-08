namespace LibplanetConsole.Node;

public readonly record struct ActionInfo
{
    private static readonly Text TypeIdKey = "type_id";

    public ActionInfo(IValue value)
    {
        if (value is Dictionary values
            && values.TryGetValue(TypeIdKey, out var typeId)
            && typeId is Text text)
        {
            TypeId = text.Value;
        }
    }

    public ActionInfo()
    {
    }

    public string TypeId { get; init; } = string.Empty;

    public TxId TxId { get; init; } = default;

    public int Index { get; init; }
}
