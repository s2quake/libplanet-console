using Bencodex.Types;

namespace OnBoarding.ConsoleHost.Serializations;

record class ActionInfo
{
    public ActionInfo(IValue value)
    {
        if (value is Dictionary values && values.ContainsKey("type_id") == true)
        {
            TypeId = $"{values["type_id"]}";
        }
    }

    public string TypeId { get; } = string.Empty;
}
