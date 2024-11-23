using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Client.Executable.EntryCommands;

[CommandSummary("Runs the libplanet-client")]
internal sealed class RunCommand
    : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandProperty]
    [CommandSummary("Specifies the port on which the client will run.")]
    [NonNegative]
    public int Port { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the private key of the client.")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console")]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the end-point of the node to connect to")]
    [EndPoint]
    public string NodeEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the file path to save logs")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    [DefaultValue("")]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the application starts without REPL")]
    public bool NoREPL { get; init; }

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        var port = Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        options.Port = port;
        options.PrivateKey = PrivateKeyUtility.ToString(privateKey);
        options.ParentProcessId = ParentProcessId;
        options.NodeEndPoint = NodeEndPoint;
        options.LogPath = GetFullPath(LogPath);
        options.NoREPL = NoREPL;

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            var application = new Application(builder);
            var port = Port is 0 ? PortUtility.NextPort() : Port;
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });
            services.AddSingleton<IConfigureOptions<ApplicationOptions>>(this);
            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }
}
