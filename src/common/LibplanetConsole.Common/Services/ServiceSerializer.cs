using System.Text.Json;
using JSSoft.Communication;

namespace LibplanetConsole.Common.Services;

internal sealed class ServiceSerializer : ISerializer
{
    public static ServiceSerializer Default { get; } = new();

    public object? Deserialize(Type type, string text)
        => JsonSerializer.Deserialize(text, type);

    public string Serialize(Type type, object? data)
        => JsonSerializer.Serialize(data, type);
}
