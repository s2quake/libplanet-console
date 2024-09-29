using System;

namespace LibplanetConsole.Console.Tests.Executable;

public class Program
{
    public static void Main(string[] args)
    {
        var delay = args.Length > 0 ? int.Parse(args[0]) : 1000;
        var dateTimeOffset = DateTimeOffset.UtcNow;
        var endDateTimeOffset = dateTimeOffset.AddMilliseconds(delay);
        while ((dateTimeOffset = DateTimeOffset.UtcNow) < endDateTimeOffset)
        {
            System.Threading.Thread.Sleep(100);
            System.Console.Out.Write("out: ");
            System.Console.Out.WriteLine(dateTimeOffset);
            System.Console.Error.Write("error: ");
            System.Console.Error.WriteLine(dateTimeOffset);
        }

        System.Console.Out.Write(dateTimeOffset.ToString("o"));
    }
}
