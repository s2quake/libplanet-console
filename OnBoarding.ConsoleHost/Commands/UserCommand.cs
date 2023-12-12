using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Games;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
sealed class UserCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly UserCollection _users;

    [ImportingConstructor]
    public UserCommand(Application application)
    {
        _application = application;
        _users = application.GetService<UserCollection>()!;
    }

    [CommandProperty(InitValue = 10)]
    public int Tick { get; set; }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _users.Count; i++)
        {
            var item = _users[i];
            tsb.Foreground = _users.CurrentUser == item ? TerminalColorType.BrightGreen : null;
            tsb.AppendLine($"[{i}]-{item.Address}");
            tsb.Foreground = null;
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.BlockIndex))]
    public void Status()
    {
        var user = _application.GetUser(IndexProperties.UserIndex);
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        var playerInfo = user.GetPlayerInfo(swarmHost, IndexProperties.BlockIndex);
        Out.WriteLineAsJson(playerInfo);
    }

    [CommandMethod]
    public void Current(int? value = null)
    {
        if (value is { } index)
        {
            _users.CurrentUser = _users[index];
        }
        else
        {
            Out.WriteLine(_users.IndexOf(_users.CurrentUser));
        }
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public void GameHistory()
    {
        var blockChain = _application.GetBlockChain(IndexProperties.SwarmIndex);
        var user = _application.GetUser(IndexProperties.UserIndex);
        var stageBlockData = GetStageRecords(blockChain, user.Address);
        var sb = new StringBuilder();
        foreach (var item in stageBlockData)
        {
            sb.AppendLine($"Block #{item.Block.Index}");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public void GamePlay()
    {
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        var user = _application.GetUser(IndexProperties.UserIndex);
        var playerInfo = user.GetPlayerInfo(swarmHost);
        var stageInfo = new StageInfo
        {
            Address = new(),
            Player = playerInfo,
            Monsters = MonsterInfo.Create(10),
        };
        var stageAction = new StageAction
        {
            StageInfo = stageInfo,
            UserAddress = user.Address,
        };
        swarmHost.StageTransaction(user, new IAction[] { stageAction });
        Out.WriteLine("Game Finished.");
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.BlockIndex))]
    [CommandMethodProperty(nameof(Tick))]
    public async Task GameReplay(CancellationToken cancellationToken)
    {
        var tick = Tick;
        var block = _application.GetBlock(IndexProperties.SwarmIndex, IndexProperties.BlockIndex);
        var user = _application.GetUser(IndexProperties.UserIndex);
        if (GetStageRecord(block, user.Address) is not { } stageBlockData)
            throw new ArgumentException($"'Block #{block.Index}' does not have {nameof(StageInfo)}.");
        var stageInfo = stageBlockData.GetStageInfo();
        var seed = stageBlockData.GetSeed();
        var stage = new Stage(stageInfo, seed, Out);
        await stage.PlayAsync(tick, cancellationToken);
        var playerInfo = (PlayerInfo)stage.Player;
        Out.WriteLineAsJson(playerInfo);
    }

    private static bool IsStageInfo(Block block)
    {
        return block
                .Transactions
                .SelectMany(x => x.Actions)
                .Any(IsStageAction);
    }

    private static bool IsStageAction(IValue value)
    {
        return value is Dictionary values && values["type_id"] is Text text && text == "stage";
    }

    private static Address GetUserAddress(IValue value)
    {
        if (value is Dictionary values && values[nameof(StageAction.UserAddress)] is { } data)
        {
            return new Address(data);
        }
        return new Address();
    }

    private static IEnumerable<StageRecord> GetStageRecords(BlockChain blockChain, Address userAddress)
    {
        for (var i = 0; i < blockChain.Count; i++)
        {
            if (GetStageRecord(blockChain[i], userAddress) is { } stageBlockData)
                yield return stageBlockData;
        }
    }

    private static StageRecord? GetStageRecord(Block block, Address userAddress)
    {
        for (var i = 0; i < block.Transactions.Count; i++)
        {
            var transaction = block.Transactions[i];
            for (var j = 0; j < transaction.Actions.Count; j++)
            {
                var action = transaction.Actions[j];
                if (IsStageAction(action) == true && GetUserAddress(action) == userAddress)
                {
                    return new(block, transaction, action, j);
                }
            }
        }
        return null;
    }

    #region StageRecord

    record class StageRecord(Block Block, ITransaction Transaction, IValue Action, int Offset)
    {
        public int GetSeed()
        {
            var block = Block;
            var transaction = Transaction;
            var offset = Offset;
            var preEvaluationHashBytes = block.PreEvaluationHash.ToByteArray();
            var signature = transaction.Signature;
            var hashedSignature = ComputeHash(signature);
            return ActionEvaluator.GenerateRandomSeed(preEvaluationHashBytes, hashedSignature, signature, offset);
        }

        public StageInfo GetStageInfo()
        {
            if (Action is Dictionary values)
                return new StageInfo((Dictionary)values[nameof(StageAction.StageInfo)]);
            throw new NotImplementedException();
        }

        private static byte[] ComputeHash(byte[] bytes)
        {
            using var hasher = SHA1.Create();
            return hasher.ComputeHash(bytes);
        }
    }

    #endregion
}
