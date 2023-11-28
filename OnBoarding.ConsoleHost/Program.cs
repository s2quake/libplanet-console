
using OnBoarding.ConsoleHost;

await using var application = new Application();
Console.WriteLine();
await application.StartAsync();
Console.WriteLine("\u001b0");
