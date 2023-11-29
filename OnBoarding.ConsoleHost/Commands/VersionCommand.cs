using System.ComponentModel.Composition;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[Export(typeof(VersionCommand))]
sealed class VersionCommand : VersionCommandBase
{
}
