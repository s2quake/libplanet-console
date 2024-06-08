namespace LibplanetConsole.Common.IO;

public sealed class TempFile : IDisposable
{
    public TempFile()
    {
        FileName = Path.GetTempFileName();
    }

    public string FileName { get; }

    public static implicit operator string(TempFile tempFile)
    {
        return tempFile.FileName;
    }

    public static TempFile WriteAllText(string content)
    {
        var tempFile = new TempFile();
        File.WriteAllText(tempFile.FileName, content);
        return tempFile;
    }

    public void Dispose()
    {
        if (File.Exists(FileName) == true)
        {
            File.Delete(FileName);
        }
    }

    public override string ToString()
    {
        return FileName;
    }
}
