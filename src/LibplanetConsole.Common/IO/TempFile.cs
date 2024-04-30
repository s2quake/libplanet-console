namespace LibplanetConsole.Common.IO;

internal sealed class TempFile : IDisposable
{
    public TempFile()
    {
        FileName = Path.GetTempFileName();
    }

    public string FileName { get; }

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
}
