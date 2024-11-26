#if LIBPLANET_NODE || LIBPLANET_CONSOLE
using LibplanetConsole.Grpc.Delegation;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE or LIBPLANET_CONSOLE must be defined.
#endif

public readonly record struct ValidatorInfo
{
    public required string Power { get; init; }

    public required string TotalShare { get; init; }

    public required bool IsJailed { get; init; }

    public required long JailedUntil { get; init; }

    public static implicit operator ValidatorInfo(ValidatorInfoProto validatorInfo)
    {
        return new ValidatorInfo
        {
            Power = validatorInfo.Power,
            TotalShare = validatorInfo.TotalShare,
            IsJailed = validatorInfo.IsJailed,
            JailedUntil = validatorInfo.JailedUntil,
        };
    }

    public static implicit operator ValidatorInfoProto(ValidatorInfo validatorInfo)
    {
        return new ValidatorInfoProto
        {
            Power = validatorInfo.Power,
            TotalShare = validatorInfo.TotalShare,
            IsJailed = validatorInfo.IsJailed,
            JailedUntil = validatorInfo.JailedUntil,
        };
    }
}

#endif // LIBPLANET_NODE || LIBPLANET_CONSOLE
