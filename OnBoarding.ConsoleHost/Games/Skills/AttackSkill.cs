using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Games.Skills;

sealed class AttackSkill(Character character, SkillInfo skillInfo)
    : SkillBase(skillInfo)
{
    private readonly Character _character = character;

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
        var @out = stage.Out;
        var query = from item in stage.Characters
                    where item.IsEnemyOf(_character) == true && item.IsDead == false
                    select item;
        var enemies = query.ToArray();
        var index = stage.Random.Next(enemies.Length);
        var amount = Value.Get(stage.Random);
        var enemy = enemies[index];
        enemy.Deal(_character, amount);
        @out.WriteLine($"'{enemy.DisplayName}({enemy.Life}/{enemy.MaxLife})' has taken '{amount}' damage from '{_character.DisplayName}'.");

        if (enemy.IsDead == true)
        {
            @out.WriteLine($"'{enemy.DisplayName}' is {TerminalStringBuilder.GetString("dead", TerminalColorType.Red)}.");
        }
    }
}
