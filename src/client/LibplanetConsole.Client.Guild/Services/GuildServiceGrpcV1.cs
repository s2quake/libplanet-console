using Grpc.Core;
using LibplanetConsole.Grpc.Guild;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Guild.Services;

internal sealed class GuildServiceGrpcV1(Guild guild)
    : GuildGrpcService.GuildGrpcServiceBase
{
    public override async Task<CreateResponse> Create(
        CreateRequest request, ServerCallContext context)
    {
        var validatorAddress = ToAddress(request.ValidatorAddress);
        var guildInfo = await guild.CreateAsync(validatorAddress, context.CancellationToken);
        return new CreateResponse { GuildInfo = guildInfo };
    }

    public override async Task<DeleteResponse> Delete(
        DeleteRequest request, ServerCallContext context)
    {
        await guild.DeleteAsync(context.CancellationToken);
        return new DeleteResponse();
    }

    public override Task<JoinResponse> Join(JoinRequest request, ServerCallContext context)
    {
        return base.Join(request, context);
    }

    public override Task<LeaveResponse> Leave(LeaveRequest request, ServerCallContext context)
    {
        return base.Leave(request, context);
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