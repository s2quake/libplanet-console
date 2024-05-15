using JSSoft.Communication;

namespace LibplanetConsole.Common.Services;

internal sealed class ServiceSerializerProvider : ISerializerProvider
{
    public static ServiceSerializerProvider Default { get; } = new();

    public string Name => "System.Text.Json";

    public ISerializer Create(IServiceContext serviceContext)
        => ServiceSerializer.Default;
}
