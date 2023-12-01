using JSSoft.Library.Terminals;

namespace OnBoarding.ConsoleHost.Games;

abstract class Character
{
    protected Character(long life)
    {
        MaxLife = life;
        Life = life;
    }

    public long MaxLife { get; }

    public long Life { get; private set; }

    public bool IsDead { get; private set; }

    public virtual string DisplayName => ToString() ?? string.Empty;

    public void Deal(long amount)
    {
        if (IsDead == true)
            throw new InvalidOperationException();
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, nameof(amount));

        Life -= amount;
        if (Life < 0)
        {
            IsDead = true;
            OnDead();
        }
    }

    public void Heal(long amount)
    {
        if (IsDead == true)
            throw new InvalidOperationException();
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, nameof(amount));

        Life = Math.Min(Life + amount, MaxLife);
    }

    public abstract bool IsEnemyOf(Character character);

    public abstract ISkill[] Skills { get; }

    public event EventHandler? Dead;

    protected virtual void OnDead()
    {
        Dead?.Invoke(this, EventArgs.Empty);
    }
}
