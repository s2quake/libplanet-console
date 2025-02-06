using Grpc.Core;
using LibplanetConsole.Common;
using LibplanetConsole.Hub.Grpc;
using LibplanetConsole.Hub.Services;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client;

internal sealed class ClientHostedService(
    IServiceProvider serviceProvider, Client client, IApplicationOptions options)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.Contents = GetClientContents(serviceProvider);
        if (options.HubUrl is { } hubUrl)
        {
            client.HubUrl = await GetNodeUrlAsync(hubUrl, cancellationToken);
            await client.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (client.IsRunning is true)
        {
            await client.StopAsync(cancellationToken);
        }
    }

    private static IClientContent[] GetClientContents(IServiceProvider serviceProvider)
    {
        var contents = serviceProvider.GetServices<IClientContent>()
            .OrderBy(item => item.Order);
        return [.. DependencyUtility.TopologicalSort(contents, content => content.Dependencies)];
    }

    private static async Task<Uri> GetNodeUrlAsync(
        Uri hubUrl, CancellationToken cancellationToken)
    {
        using var hubChannel = HubChannel.CreateChannel(hubUrl);
        var service = new HubService(hubChannel);
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var request = new GetServiceRequest
        {
            ServiceName = "libplanet.console.node.v1",
        };
        var response = await service.GetServiceAsync(request, callOptions);
        return new Uri(response.Url);
    }
}
