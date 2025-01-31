namespace LibplanetConsole.Console.Seed;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSeed(this IServiceCollection @this)
    {
        @this.AddSingleton<SeedService>()
            .AddSingleton<ISeedService>(s => s.GetRequiredService<SeedService>());

        @this.AddHostedService<SeedHostedService>();

        return @this;
    }
}
