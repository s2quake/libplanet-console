using System.ComponentModel.Composition;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class ApplicationOptions
{
    [CommandProperty(InitValue = 1)]
    public int SwarmCount { get; set; }

    [CommandProperty(InitValue = 10)]
    public int UserCount { get; set; }
}
