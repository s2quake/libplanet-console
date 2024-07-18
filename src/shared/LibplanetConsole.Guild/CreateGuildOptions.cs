using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class CreateGuildOptions : OptionsBase<CreateGuildOptions>
{
    public required string Name { get; init; }
}
