using System.Diagnostics;
using System.Text.Json;
using LibplanetConsole.Common.IO;

namespace LibplanetConsole.Common;

public static class JsonUtility
{
    private static readonly bool IsJQSupported = SupportsJQ();
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
    };

    public static string SerializeObject(object value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static string SerializeObject(object value, bool isColorized)
    {
        var json = SerializeObject(value);
        if (isColorized == true && IsJQSupported == true)
        {
            using var tempFile = TempFile.WriteAllText(json);
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = $"jq";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = $". -C \"{tempFile.FileName}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        return json;
    }

    public static T DeserializeObject<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value)!;
    }

    private static bool SupportsJQ()
    {
        try
        {
            if (OperatingSystem.IsMacOS() == true)
            {
                var process = new Process();
                process.StartInfo.FileName = $"which";
                process.StartInfo.Arguments = "jq";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += (s, e) => { };
                process.ErrorDataReceived += (s, e) => { };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }
        catch
        {
            // ignored
        }

        return false;
    }
}
