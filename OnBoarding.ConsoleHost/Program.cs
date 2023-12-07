using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost;

var @out = Console.Out;
@out.WriteLine(TerminalStringBuilder.GetString("Welcome to jeesu world for OnBoarding.", TerminalColorType.BrightGreen));
await using var application = new Application();
@out.WriteLine();
await application.StartAsync(args);
@out.WriteLine("\u001b0");
