using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Numerics;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action.Sys;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Types.Consensus;
using Libplanet.Types.Tx;

namespace OnBoarding.ConsoleHost.Commands;

// [Export(typeof(ICommand))]
// [method: ImportingConstructor]
// sealed class TransactionCommand(Swarm swarm, PrivateKey privateKey) : CommandMethodBase("tx")
// {
//     private readonly Swarm _swarm = swarm;
//     private readonly PrivateKey _privateKey = privateKey;

//     [CommandMethod]
//     public void Create()
//     {
//         var validatorList = new List<Validator>
//         {
//             new(_privateKey.PublicKey, BigInteger.One),
//         };
//         var validatorSet = new ValidatorSet(validatorList);
//         var nonce = 0L;
//         var action = new Initialize(
//             validatorSet: validatorSet,
//             states: ImmutableDictionary.Create<Address, IValue>()
//             );
//         var transaction = Transaction.Create(
//             nonce,
//             privateKey: _privateKey,
//             genesisHash: null,
//             actions: [action.PlainValue],
//             timestamp: DateTimeOffset.MinValue
//             );
//         _swarm.BroadcastTxs(txs: [transaction]);
//     }
// }
