using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace LibplanetConsole.Console;

public static class WebApplicationBuilderExtensions
{
    public static void ListenConsole(
        this WebApplicationBuilder @this, IConfiguration configuration)
    {
        var port = GetPort(configuration);
        @this.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
            options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        });
    }

    private static int GetPort(IConfiguration configuration)
        => configuration.GetValue<int>($"{ApplicationOptions.Position}:Port");
}
