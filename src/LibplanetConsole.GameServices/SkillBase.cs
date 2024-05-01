using LibplanetConsole.GameServices.Serializations;

namespace LibplanetConsole.GameServices;

public abstract class SkillBase(SkillInfo skillInfo) : ISkill
{
    private EventHandler? _canExecuteChangedEvent;

    event EventHandler? ISkill.CanExecuteChanged
    {
        add => _canExecuteChangedEvent += value;
        remove => _canExecuteChangedEvent -= value;
    }

    public ValueRange Value { get; } = skillInfo.Value;

    public long CoolTime { get; set; } = skillInfo.CoolTime;

    public long MaxCoolTime { get; } = skillInfo.MaxCoolTime;

    public void Reset()
    {
        CoolTime = 0;
    }

    void ISkill.Tick()
    {
        CoolTime = Math.Max(0, CoolTime - 1);
    }

    bool ISkill.CanExecute(Stage stage) => OnCanExecute(stage);

    void ISkill.Execute(Stage stage)
    {
        OnExecute(stage);
        CoolTime = MaxCoolTime;
    }

    protected virtual bool OnCanExecute(Stage stage) => CoolTime == 0;

    protected abstract void OnExecute(Stage stage);

    protected void InvokeCanExecuteChangedEvent()
        => _canExecuteChangedEvent?.Invoke(this, EventArgs.Empty);
}
