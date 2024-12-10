namespace LibplanetConsole.Common.IO;

public static class FileUtility
{
    public static void DeleteIfExists(string path)
    {
        if (File.Exists(path) == true)
        {
            File.Delete(path);
        }
    }

    public static bool TryDelete(string path)
    {
        try
        {
            if (File.Exists(path) == true)
            {
                File.Delete(path);
                return true;
            }
        }
        catch
        {
            // ignored
        }

        return false;
    }
}
