using JSSoft.Commands;
using LibplanetConsole.Client.Commands;
using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClient(
        this IServiceCollection @this, ApplicationOptions options)
    {
        var synchronizationContext = SynchronizationContext.Current ?? new();
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
        @this.AddSingleton(synchronizationContext);
        @this.AddSingleton(options);

        @this.AddSingleton<Client>()
             .AddSingleton<IClient>(s => s.GetRequiredService<Client>())
             .AddSingleton<IBlockChain>(s => s.GetRequiredService<Client>());
        @this.AddHostedService<ClientHostedService>();
        @this.AddSingleton<IInfoProvider, ApplicationInfoProvider>();
        @this.AddSingleton<IInfoProvider, ClientInfoProvider>();

        @this.AddSingleton<ICommand, AddressCommand>();
        @this.AddSingleton<ICommand, ExitCommand>();
        @this.AddSingleton<ICommand, InfoCommand>();
        @this.AddSingleton<ICommand, KeyCommand>();
        @this.AddSingleton<ICommand, StartCommand>();
        @this.AddSingleton<ICommand, StopCommand>();
        @this.AddSingleton<ICommand, TxCommand>();
        return @this;
    }
}
