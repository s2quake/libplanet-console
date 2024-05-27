using JSSoft.Terminals;

namespace LibplanetConsole.Common.Extensions;

public static class TextWriterExtensions
{
    public static void WriteLineAsJson(this TextWriter @this, object obj)
    {
        var json = JsonUtility.SerializeObject(obj, isColorized: true);
        @this.WriteLine(json);
    }

    public static Task WriteLineAsJsonAsync(this TextWriter @this, object obj)
    {
        var json = JsonUtility.SerializeObject(obj, isColorized: true);
        return @this.WriteLineAsync(json);
    }

    public static void WriteColoredLine(
        this TextWriter @this, string text, TerminalColorType colorType)
    {
        @this.WriteLine(TerminalStringBuilder.GetString(text, colorType));
    }

    public static void WriteSeparator(this TextWriter @this, int length)
    {
        @this.WriteLine(new string('=', length));
    }

    public static void WriteSeparator(
        this TextWriter @this, int length, TerminalColorType colorType)
    {
        var text = new string('=', length);
        @this.WriteLine(TerminalStringBuilder.GetString(text, colorType));
    }

    public static void WriteSeparator(this TextWriter @this, TerminalColorType colorType)
    {
        WriteSeparator(@this, length: 80, colorType);
    }

    public static void WriteLineIf(this TextWriter @this, bool condition, string message)
    {
        if (condition == true)
        {
            @this.WriteLine(message);
        }
    }
}
