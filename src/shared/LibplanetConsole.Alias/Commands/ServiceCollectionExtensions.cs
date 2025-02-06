using JSSoft.Commands;

namespace LibplanetConsole.Alias.Commands;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAliasCommands(
        this IServiceCollection @this)
    {
        @this.AddSingleton<AliasCommand>()
            .AddSingleton<ICommand>(s => s.GetRequiredService<AliasCommand>());
        @this.AddSingleton<ICommand, AliasListCommand>();
        @this.AddSingleton<ICommand, AliasAddCommand>();
        @this.AddSingleton<ICommand, AliasRemoveCommand>();
        @this.AddSingleton<ICommand, AliasUpdateCommand>();
        return @this;
    }
}
