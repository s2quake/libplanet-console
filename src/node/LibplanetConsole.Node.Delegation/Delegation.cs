using Libplanet.Action.State;
using LibplanetConsole.Delegation;
using LibplanetConsole.Framework;
using Nekoyume;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Delegation;
using Nekoyume.Model.State;

namespace LibplanetConsole.Node.Delegation;

[Export]
[Export(typeof(IDelegation))]
[Export(typeof(IApplicationService))]
internal sealed class Delegation(
    IApplication application, INode node, IBlockChain blockChain)
    : IDelegation, IApplicationService, IAsyncDisposable
{
    private const int Timeout = 10000;
    private Currency? _goldCurrency;
    private readonly INode _node = node;
    private readonly IBlockChain blockChain = blockChain;
    private readonly AutoResetEvent _startedEvent = new(false);
    private DelegateeInfo _delegateeInfo;

    public DelegateeInfo Info => _delegateeInfo;

    public async Task<DelegateeInfo> PromoteAsync(
        double amount, CancellationToken cancellationToken)
    {
        if (_goldCurrency is null)
        {
            throw new InvalidOperationException("Gold currency is not initialized.");
        }

        var publicKey = _node.PublicKey;
        var gold = ToGold(amount);
        var actions = new IAction[]
        {
            new PromoteValidator(publicKey, gold),
        };
        await _node.SendTransactionAsync(actions, cancellationToken);
        return new DelegateeInfo();
    }

    public async Task ClaimAsync(Address nodeAddress, CancellationToken cancellationToken)
    {
        if (_goldCurrency is null)
        {
            throw new InvalidOperationException("Gold currency is not initialized.");
        }

        var validatorDelegatee = nodeAddress;
        var actions = new IAction[]
        {
            new ClaimRewardValidator(validatorDelegatee),
        };
        await _node.SendTransactionAsync(actions, cancellationToken);
    }

    public async Task<BondInfo> DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var validatorDelegatee = nodeAddress;
        var gold = ToGold(amount);
        var actions = new IAction[]
        {
            new DelegateValidator(validatorDelegatee, gold),
        };
        await _node.SendTransactionAsync(actions, cancellationToken);
        return new BondInfo();
    }

    public async Task<DelegateeInfo> UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        // var validatorAddress = nodeAddress.DeriveAsValidator();
        // var actions = new IAction[]
        // {
        //     new Nekoyume.Action.DPoS.Undelegate
        //     {
        //         Validator = validatorAddress,
        //         ShareAmount = Nekoyume.Action.DPoS.Misc.Asset.Share * shareAmount,
        //     },
        // };
        // await node.AddTransactionAsync(actions, cancellationToken);
        // return new ValidatorInfo(node.GetService<BlockChain>().GetWorldState(), node.Address);
        throw new NotImplementedException();
    }

    public async Task<DelegateeInfo> RedelegateAsync(
        Address srcNodeAddress,
        Address destNodeAddress,
        long shareAmount,
        CancellationToken cancellationToken)
    {
        // var srcAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(srcNodeAddress);
        // var destAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(destNodeAddress);
        // var actions = new IAction[]
        // {
        //     new Nekoyume.Action.DPoS.Redelegate
        //     {
        //         SrcValidator = srcAddress,
        //         DstValidator = destAddress,
        //         ShareAmount = Nekoyume.Action.DPoS.Misc.Asset.Share * shareAmount,
        //     },
        // };
        // await node.AddTransactionAsync(actions, cancellationToken);
        // return new ValidatorInfo(node.GetService<BlockChain>().GetWorldState(), node.Address);
        throw new NotImplementedException();
    }

    public async Task<DelegateeInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // return await application.InvokeAsync(Func, cancellationToken);

        // ValidatorInfo[] Func()
        // {
        //     var worldState = node.GetService<BlockChain>().GetWorldState();
        //     var validatorSet = worldState.GetValidatorSet();
        //     var validators = validatorSet.Validators;
        //     return [.. validators.Select(GetValidatorInfo)];

        //     ValidatorInfo GetValidatorInfo(Validator validator)
        //         => new(worldState, validator.OperatorAddress);
        // }
    }

    public async Task<DelegateeInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // return await application.InvokeAsync(Func, cancellationToken);

        // ValidatorInfo Func()
        // {
        //     var worldState = node.GetService<BlockChain>().GetWorldState();
        //     var validatorSet = worldState.GetValidatorSet();
        //     var validators = validatorSet.Validators;
        //     var validator = validators.First(item => item.OperatorAddress == (Address)nodeAddress);
        //     return new ValidatorInfo(worldState, validator.OperatorAddress);
        // }
    }

    public async Task<FungibleAssetValue> GetRewardPoolAsync(
        Address nodeAddress, CancellationToken cancellationToken)
    {
        if (_goldCurrency is not { } goldCurrency)
        {
            throw new InvalidOperationException("Gold currency is not initialized.");
        }

        var stateAddress =
                DelegationAddress.DelegateeMetadataAddress(_node.Address, Addresses.ValidatorDelegatee);
        IValue value = await blockChain.GetStateAsync(Addresses.ValidatorDelegateeMetadata, stateAddress, cancellationToken);
        var metadata = new DelegateeMetadata(_node.Address, Addresses.ValidatorDelegatee, value);
        return await blockChain.GetBalanceAsync(metadata.RewardDistributorAddress, goldCurrency, cancellationToken);
    }

    Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        _node.Started += Node_Started;
        _node.Stopped += Node_Stoped;
        return Task.CompletedTask;
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        _node.Started -= Node_Started;
        _node.Stopped -= Node_Stoped;
        return ValueTask.CompletedTask;
    }

    private FungibleAssetValue ToGold(double amount)
    {
        if (_goldCurrency is not { } goldCurrency)
        {
            throw new InvalidOperationException("Gold currency is not initialized.");
        }

        return FungibleAssetValue.Parse(goldCurrency, $"{amount}");
    }

    private async Task<Currency> GetCurrencyAsync(CancellationToken cancellationToken)
    {
        if (_goldCurrency is not { } goldCurrency)
        {
            var state = await blockChain.GetStateAsync(
                ReservedAddresses.LegacyAccount, GoldCurrencyState.Address, cancellationToken);
            goldCurrency = new GoldCurrencyState((Dictionary)state).Currency;
            _goldCurrency = goldCurrency;
        }

        return goldCurrency;
    }

    private async void Node_Started(object? sender, EventArgs e)
    {
        using var cancellationTokenSource = new CancellationTokenSource(Timeout);
        try
        {
            _goldCurrency = await GetCurrencyAsync(cancellationTokenSource.Token);

            var stateAddress =
                DelegationAddress.DelegateeMetadataAddress(_node.Address, Addresses.ValidatorDelegatee);
            IValue value = await blockChain.GetStateAsync(Addresses.ValidatorDelegateeMetadata, stateAddress, cancellationTokenSource.Token);
            var metadata = new DelegateeMetadata(_node.Address, Addresses.ValidatorDelegatee, value);
            _delegateeInfo = new DelegateeInfo(metadata);
            _startedEvent.Set();
        }
        catch (OperationCanceledException)
        {
            _goldCurrency = null;
            _startedEvent.Set();
        }
    }

    private void Node_Stoped(object? sender, EventArgs e)
    {
        _startedEvent.WaitOne(Timeout);
        _goldCurrency = null;
        _startedEvent.Reset();
    }

}
