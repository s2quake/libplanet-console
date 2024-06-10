using Bencodex.Types;

namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct ActionInfo
{
    public ActionInfo(IValue value)
    {
        if (value is Dictionary values && values.ContainsKey("type_id") == true)
        {
            TypeId = $"{values["type_id"]}";
        }
    }

    public ActionInfo()
    {
    }

    public string TypeId { get; init; } = string.Empty;
}
