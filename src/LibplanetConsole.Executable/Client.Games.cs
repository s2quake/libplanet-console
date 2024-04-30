using Bencodex.Types;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Games;
using LibplanetConsole.Games.Serializations;

namespace LibplanetConsole.Executable;

internal sealed partial class Client
{
    public PlayerInfo? PlayerInfo { get; set; }

    public static PlayerInfo? GetPlayerInfo(BlockChain blockChain, Address address)
    {
        var worldState = blockChain.GetWorldState();
        var account = worldState.GetAccountState(address);
        if (account.GetState(UserStates.PlayerInfo) is Dictionary values)
        {
            return new PlayerInfo(values);
        }

        return null;
    }

    public void Login(BlockChain blockChain)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline == true, $"{this} is already online.");

        PlayerInfo = GetPlayerInfo(blockChain, Address);
        IsOnline = true;
        Out.WriteLine($"{this} is logged in.");
    }

    public void Logout()
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");

        PlayerInfo = null;
        IsOnline = false;
        Out.WriteLine($"{this} is logged out.");
    }

    public PlayerInfo GetPlayerInfo()
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: PlayerInfo == null,
            message: $"{this} does not have character.");

        return PlayerInfo!;
    }

    public GamePlayRecord[] GetGameHistory(BlockChain blockChain)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");

        return GamePlayRecord.GetGamePlayRecords(blockChain, Address).ToArray();
    }

    public void Refresh(BlockChain blockChain)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");

        PlayerInfo = GetPlayerInfo(blockChain, Address);
    }

    public Task<long> PlayGameAsync(BlockChain blockChain, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task ReplayGameAsync(
        BlockChain blockChain, int tick, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task ReplayGameAsync(
        BlockChain blockChain, long blockIndex, int tick, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task CreateCharacterAsync(Client client, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task ReviveCharacterAsync(Client client, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
