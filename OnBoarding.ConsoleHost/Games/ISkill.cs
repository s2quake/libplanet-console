using System.Collections;
using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

interface ISkill
{
    void Tick();

    bool CanExecute(Stage stage);

    void Execute(Stage stage);

    event EventHandler? CanExecuteChanged;
}
