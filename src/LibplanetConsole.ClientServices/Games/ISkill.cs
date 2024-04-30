namespace LibplanetConsole.ClientServices.Games;

public interface ISkill
{
    event EventHandler? CanExecuteChanged;

    void Tick();

    bool CanExecute(Stage stage);

    void Execute(Stage stage);
}
