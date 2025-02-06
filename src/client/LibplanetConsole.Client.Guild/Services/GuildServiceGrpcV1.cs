using Grpc.Core;
using LibplanetConsole.Guild.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Guild.Services;

internal sealed class GuildServiceGrpcV1(IGuild guild)
    : GuildGrpcService.GuildGrpcServiceBase
{
    public override async Task<CreateResponse> Create(
        CreateRequest request, ServerCallContext context)
    {
        var validatorAddress = ToAddress(request.ValidatorAddress);
        await guild.CreateAsync(validatorAddress, context.CancellationToken);
        return new CreateResponse();
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

    public override async Task<MoveResponse> Move(MoveRequest request, ServerCallContext context)
    {
        var guildAddress = ToAddress(request.GuildAddress);
        await guild.MoveAsync(guildAddress, context.CancellationToken);
        return new MoveResponse();
    }

    public override async Task<BanResponse> Ban(BanRequest request, ServerCallContext context)
    {
        var memberAddress = ToAddress(request.MemberAddress);
        await guild.BanAsync(memberAddress, context.CancellationToken);
        return new BanResponse();
    }

    public override async Task<UnbanResponse> Unban(UnbanRequest request, ServerCallContext context)
    {
        var memberAddress = ToAddress(request.MemberAddress);
        await guild.UnbanAsync(memberAddress, context.CancellationToken);
        return new UnbanResponse();
    }

    public override async Task<ClaimResponse> Claim(ClaimRequest request, ServerCallContext context)
    {
        await guild.ClaimAsync(context.CancellationToken);
        return new ClaimResponse();
    }

    public override async Task<GetInfoResponse> GetInfo(
        GetInfoRequest request, ServerCallContext context)
    {
        var memberAddress = ToAddress(request.MemberAddress);
        var info = await guild.GetInfoAsync(memberAddress, context.CancellationToken);
        return new GetInfoResponse { GuildInfo = info };
    }
}
