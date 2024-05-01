using Bencodex.Types;

namespace LibplanetConsole.NodeServices.Serializations;

public record class ActionInfo
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
