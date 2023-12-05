namespace OnBoarding.ConsoleHost.Games;

interface ISkill
{
    void Tick();

    bool CanExecute(Stage stage);

    void Execute(Stage stage);

    event EventHandler? CanExecuteChanged;
}
