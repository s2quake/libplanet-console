using System.Runtime.Serialization;
using JSSoft.Library.Terminals;
using Libplanet.Crypto;
using Libplanet.Stun.Attributes;

namespace OnBoarding.ConsoleHost.Games;

abstract class Character
{
    protected Character(CharacterInfo characterInfo)
    {
        Name = characterInfo.Name;
        MaxLife = characterInfo.MaxLife;
        Life = characterInfo.Life;
    }

    public string Name { get; }

    public long MaxLife { get; }

    public long Life { get; private set; }

    public bool IsDead { get; private set; }

    public virtual string DisplayName => ToString() ?? string.Empty;

    public abstract ISkill[] Skills { get; }

    public void Deal(Character attacker, long amount) => OnDeal(attacker, amount);

    public void Heal(long amount)
    {
        if (IsDead == true)
            throw new InvalidOperationException();
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, nameof(amount));

        Life = Math.Min(Life + amount, MaxLife);
    }

    public void Revive()
    {
        if (IsDead == false)
            throw new InvalidOperationException("Player did not die.");
        Life = (long)(MaxLife * 0.25);
        IsDead = false;

        Revived?.Invoke(this, EventArgs.Empty);
    }

    public abstract bool IsEnemyOf(Character character);

    public event EventHandler? Dead;

    public event EventHandler? Revived;

    protected virtual void OnDeal(Character attacker, long amount)
    {
        if (IsDead == true)
            throw new InvalidOperationException();
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, nameof(amount));

        var life = Life;
        Life -= amount;
        if (Life < 0)
        {
            IsDead = true;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        Dead?.Invoke(this, EventArgs.Empty);
    }
}
