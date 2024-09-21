using JSSoft.Terminals;

namespace LibplanetConsole.Common.Extensions;

public static class TextWriterExtensions
{
    public static void WriteLineAsJson(this TextWriter @this, object obj)
    {
        var json = JsonUtility.Serialize(obj, isColorized: true);
        @this.WriteLine(json);
    }

    public static async Task WriteLineAsJsonAsync(
        this TextWriter @this, object obj, CancellationToken cancellationToken)
    {
        var json = await JsonUtility.SerializeAsync(obj, isColorized: true, cancellationToken);
        await @this.WriteLineAsync(json);
    }

    public static void WriteColoredLine(
        this TextWriter @this, string text, TerminalColorType colorType)
    {
        @this.WriteLine(TerminalStringBuilder.GetString(text, colorType));
    }

    public static Task WriteColoredLineAsync(
        this TextWriter @this, string text, TerminalColorType colorType)
    {
        return @this.WriteLineAsync(TerminalStringBuilder.GetString(text, colorType));
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

    public static Task WriteSeparatorAsync(this TextWriter @this, int length)
    {
        return @this.WriteLineAsync(new string('=', length));
    }

    public static Task WriteSeparatorAsync(
        this TextWriter @this, int length, TerminalColorType colorType)
    {
        var text = new string('=', length);
        return @this.WriteLineAsync(TerminalStringBuilder.GetString(text, colorType));
    }

    public static Task WriteSeparatorAsync(this TextWriter @this, TerminalColorType colorType)
    {
        return WriteSeparatorAsync(@this, length: 80, colorType);
    }

    public static void WriteLineIf(this TextWriter @this, bool condition, string message)
    {
        if (condition == true)
        {
            @this.WriteLine(message);
        }
    }

    public static async Task WriteLineIfAsync(this TextWriter @this, bool condition, string message)
    {
        if (condition == true)
        {
            await @this.WriteLineAsync(message);
        }
    }

    public static void WriteActionIf(
        this TextWriter @this, bool condition, Action<TextWriter> action)
    {
        if (condition == true)
        {
            action(@this);
        }
    }
}
