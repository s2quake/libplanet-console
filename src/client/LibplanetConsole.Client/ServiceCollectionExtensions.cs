using LibplanetConsole.Client.Commands;
using LibplanetConsole.Common;
// using LibplanetConsole.Common.Services;
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

        @this.AddSingleton(s => new Client(s, options))
             .AddSingleton<IClient>(s => s.GetRequiredService<Client>())
             .AddSingleton<IBlockChain>(s => s.GetRequiredService<Client>());
        @this.AddHostedService<ClientHostedService>();
     //    @this.AddSingleton<ClientService>()
          //    .AddSingleton<ILocalService>(s => s.GetRequiredService<ClientService>())
          //    .AddSingleton<IClientService>(s => s.GetRequiredService<ClientService>());
     //    @this.AddSingleton<ClientServiceContext>();
     //    @this.AddSingleton<RemoteBlockChainService>();
          //    .AddSingleton<IRemoteService>(s => s.GetRequiredService<RemoteBlockChainService>());
     //    @this.AddSingleton<RemoteNodeContext>();
     //    @this.AddSingleton<RemoteNodeService>();
          //    .AddSingleton<IRemoteService>(s => s.GetRequiredService<RemoteNodeService>());
        @this.AddSingleton<IInfoProvider, ApplicationInfoProvider>();
        @this.AddSingleton<IInfoProvider, ClientInfoProvider>();

        @this.AddSingleton<AddressCommand>();
        @this.AddSingleton<ExitCommand>();
        @this.AddSingleton<InfoCommand>();
        @this.AddSingleton<KeyCommand>();
        @this.AddSingleton<StartCommand>();
        @this.AddSingleton<StopCommand>();
        @this.AddSingleton<TxCommand>();
        return @this;
    }
}
