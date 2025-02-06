using Grpc.Core;
using LibplanetConsole.Hand;
using LibplanetConsole.Hand.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Hand.Services;

internal sealed class HandServiceGrpcV1(IHand hand)
    : HandGrpcService.HandGrpcServiceBase
{
}
