using Grpc.Core;
using LibplanetConsole.Grpc.Guild;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Guild.Services;

internal sealed class GuildServiceGrpcV1(IGuild guild)
    : GuildGrpcService.GuildGrpcServiceBase
{
    public override async Task<CreateResponse> Create(
        CreateRequest request, ServerCallContext context)
    {
        var guildInfo = await guild.CreateAsync(context.CancellationToken);
        return new CreateResponse { GuildInfo = guildInfo };
    }

    public override async Task<DeleteResponse> Delete(
        DeleteRequest request, ServerCallContext context)
    {
        await guild.DeleteAsync(context.CancellationToken);
        return new DeleteResponse();
    }

    public override async Task<JoinResponse> Join(JoinRequest request, ServerCallContext context)
    {
        var guildAddress = ToAddress(request.GuildAddress);
        await guild.JoinAsync(guildAddress, context.CancellationToken);
        return new JoinResponse();
    }

    public override async Task<LeaveResponse> Leave(LeaveRequest request, ServerCallContext context)
    {
        await guild.LeaveAsync(context.CancellationToken);
        return new LeaveResponse();
    }

    public override async Task<BanResponse> Ban(BanRequest request, ServerCallContext context)
    {
        var memberAddress = ToAddress(request.MemberAddress);
        await guild.BanMemberAsync(memberAddress, context.CancellationToken);
        return new BanResponse();
    }

    public override async Task<UnbanResponse> Unban(UnbanRequest request, ServerCallContext context)
    {
        var memberAddress = ToAddress(request.MemberAddress);
        await guild.UnbanMemberAsync(memberAddress, context.CancellationToken);
        return new UnbanResponse();
    }

    public override Task<MoveResponse> Move(MoveRequest request, ServerCallContext context)
    {
        return base.Move(request, context);
    }
}
