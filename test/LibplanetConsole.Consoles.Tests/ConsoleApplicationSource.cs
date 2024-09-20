using System;

namespace LibplanetConsole.Consoles.Tests.Executable;

public class Program
{
    public static void Main(string[] args)
    {
        var delay = args.Length > 0 ? int.Parse(args[0]) : 1000;
        var dateTimeOffset = DateTimeOffset.UtcNow;
        while (DateTimeOffset.UtcNow < dateTimeOffset.AddMilliseconds(delay))
        {
            System.Threading.Thread.Sleep(100);
            Console.Out.WriteLine(dateTimeOffset);
            Console.Error.WriteLine(dateTimeOffset);
        }
    }
}
