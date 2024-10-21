using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Logging;
using LibplanetConsole.Node.Commands;
using LibplanetConsole.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node;

public static class WebApplicationBuilderExtensions
{
    public static void ListenNode(
        this WebApplicationBuilder @this, IConfiguration configuration)
    {
    }
}
