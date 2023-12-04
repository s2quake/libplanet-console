using JSSoft.Library.Terminals;

namespace OnBoarding.ConsoleHost.Games;

sealed class AttackSkill(Character character, long maxCoolTime) : SkillBase(maxCoolTime)
{
    private readonly Character _character = character;
    private ValueRange _damage = ValueRange.FromBeginEnd(1, 3);

    protected override bool OnCanExecute(Stage stage)
    {
        var query = from item in stage.Characters
                    where item.IsEnemyOf(_character) == true && item.IsDead == false
                    select item;
        if (query.Any() == false)
            return false;
        return base.OnCanExecute(stage);
    }

    protected override void OnExecute(Stage stage)
    {
        var query = from item in stage.Characters
                    where item.IsEnemyOf(_character) == true && item.IsDead == false
                    select item;
        var enemies = query.ToArray();
        var index = RandomUtility.GetNext(enemies.Length);
        var amount = _damage.Get();
        var enemy = enemies[index];
        enemy.Deal(_character, amount);
        Console.WriteLine($"'{enemy.DisplayName}({enemy.Life}/{enemy.MaxLife})' has taken '{amount}' damage from '{_character.DisplayName}'.");

        if (enemy.IsDead == true)
        {
            Console.WriteLine($"'{enemy.DisplayName}' is {TerminalStringBuilder.GetString("dead", TerminalColorType.Red)}.");
        }
    }
}
