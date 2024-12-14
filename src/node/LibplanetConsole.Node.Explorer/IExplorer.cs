using LibplanetConsole.Common;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Explorer;

public interface IExplorer
{
    Uri Url { get; }
}
