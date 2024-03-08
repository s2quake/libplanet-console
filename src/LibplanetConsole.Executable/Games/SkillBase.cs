using LibplanetConsole.Executable.Games.Serializations;

namespace LibplanetConsole.Executable.Games;

abstract class SkillBase(SkillInfo skillInfo) : ISkill
{
    private EventHandler? _canExecuteChangedEvent;

    public ValueRange Value { get; } = skillInfo.Value;

    public long CoolTime { get; set; } = skillInfo.CoolTime;

    public long MaxCoolTime { get; } = skillInfo.MaxCoolTime;

    public void Reset()
    {
        CoolTime = 0;
    }

    protected virtual bool OnCanExecute(Stage stage) => CoolTime == 0;

    protected abstract void OnExecute(Stage stage);

    protected void InvokeCanExecuteChangedEvent() => _canExecuteChangedEvent?.Invoke(this, EventArgs.Empty);

    #region ISkill

    event EventHandler? ISkill.CanExecuteChanged
    {
        add => _canExecuteChangedEvent += value;
        remove => _canExecuteChangedEvent -= value;
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

    #endregion
}
