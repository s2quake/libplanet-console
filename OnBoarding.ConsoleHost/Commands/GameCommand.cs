using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using Newtonsoft.Json;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Games;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class GameCommand(Application application) : CommandMethodBase
{
    private readonly BlockChain _blockChain = application.GetService<BlockChain>()!;
    private readonly UserCollection _users = application.GetService<UserCollection>()!;

    [CommandProperty(InitValue = 1)]
    public int Tick { get; set; }

    public bool CanPlay => Player.GetPlayerInfo(_blockChain, Player.CurrentAddress).Life > 0;

    [CommandMethod]
    [CommandMethodValidation(nameof(CanPlay))]
    public void Play()
    {
        var playerInfo = Player.GetPlayerInfo(_blockChain, Player.CurrentAddress);
        var user = _users.First(item => item.Address == playerInfo.Address);
        var stageInfo = new StageInfo
        {
            Address = new(),
            Player = playerInfo,
            Monsters = MonsterInfo.Create(10),
        };
        var stageAction = new StageAction
        {
            StageInfo = stageInfo,
        };
        BlockChainUtils.AppendNew(_blockChain, user, _users, [stageAction]);
        Out.WriteLine("Game Finished.");
    }

    [CommandMethod]
    public async Task Replay(int blockIndex, CancellationToken cancellationToken)
    {
        var block = _blockChain[blockIndex];
        var (stageInfo, seed) = GetStageInfo(block);
        var stage = new Stage(stageInfo, seed, Out);
        await stage.StartAsync(10, cancellationToken);
        var playerInfo = (PlayerInfo)stage.Player;
        Out.WriteLine(JsonConvert.SerializeObject(playerInfo, Formatting.Indented));
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _blockChain.Count; i++)
        {
            var block = _blockChain[i];
            if (IsStageInfo(block) == true)
            {
                sb.AppendLine($"Block #{i}");
            }
        }
        Out.Write(sb.ToString());
    }

    private static bool IsStageInfo(Block block)
    {
        var query = from transaction in block.Transactions
                    from action in transaction.Actions
                    where IsStageAction(action) == true
                    select action;
        return query.Any() == true;
    }

    private static bool IsStageAction(IValue value)
    {
        return value is Dictionary values && values["type_id"] is Text text && text == "stage";
    }

    private static (ITransaction transaction, IValue aciton, int offset) GetStageInfoAction(Block block)
    {
        for (var i = 0; i < block.Transactions.Count; i++)
        {
            var transaction = block.Transactions[i];
            for (var j = 0; j < transaction.Actions.Count; j++)
            {
                var action = transaction.Actions[j];
                if (IsStageAction(action) == true)
                {
                    return (transaction, action, j);
                }
            }
        }
        throw new ArgumentException($"'Block #{block.Index}' does not have {nameof(StageInfo)}.");
    }

    private static (StageInfo stageInfo, int seed) GetStageInfo(Block block)
    {
        var (transaction, action, offset) = GetStageInfoAction(block);
        var values = (Dictionary)action;
        var stageInfo = new StageInfo((Dictionary)values["StageInfo"]);
        var preEvaluationHashBytes = block.PreEvaluationHash.ToByteArray();
        var signature = transaction.Signature;
        var hashedSignature = ComputeHash(signature);
        var seed = ActionEvaluator.GenerateRandomSeed(preEvaluationHashBytes, hashedSignature, signature, offset);
        return (stageInfo, seed);

        static byte[] ComputeHash(byte[] bytes)
        {
            using var hasher = SHA1.Create();
            return hasher.ComputeHash(bytes);
        }
    }
}
