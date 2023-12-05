namespace OnBoarding.ConsoleHost.Games;

abstract class SkillBase : ISkill
{
    protected SkillBase(SkillInfo skillInfo)
    {
        MaxCoolTime = skillInfo.MaxCoolTime;
        CoolTime = skillInfo.CoolTime;
        Value = skillInfo.Value;
    }

    private EventHandler? _canExecuteChangedEvent;

    public ValueRange Value { get; }

    public long CoolTime { get; set; }

    public long MaxCoolTime { get; }

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
