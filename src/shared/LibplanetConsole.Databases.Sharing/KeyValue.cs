namespace LibplanetConsole.Databases;

public readonly record struct KeyValue
{
    public KeyValue(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; init; }

    public string Value { get; init; }
}
