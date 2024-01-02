using OnBoarding.ConsoleHost.Exceptions;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Games;

abstract class Character(CharacterInfo characterInfo)
{
    public string Name { get; } = characterInfo.Name;

    public long MaxLife { get; } = characterInfo.MaxLife;

    public long Life { get; private set; } = characterInfo.Life;

    public bool IsDead { get; private set; }

    public virtual string DisplayName => ToString() ?? string.Empty;

    public abstract ISkill[] Skills { get; }

    public void Deal(Character attacker, long amount) => OnDeal(attacker, amount);

    public void Heal(long amount)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsDead == true, "Player has died.");
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        Life = Math.Min(Life + amount, MaxLife);
    }

    public void Revive()
    {
        InvalidOperationExceptionUtility.ThrowIf(IsDead != true, "Player did not die.");

        Life = (long)(MaxLife * 0.25);
        IsDead = false;

        Revived?.Invoke(this, EventArgs.Empty);
    }

    public abstract bool IsEnemyOf(Character character);

    public event EventHandler? Dead;

    public event EventHandler? Revived;

    protected virtual void OnDeal(Character attacker, long amount)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsDead == true, "Player has died.");
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

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
