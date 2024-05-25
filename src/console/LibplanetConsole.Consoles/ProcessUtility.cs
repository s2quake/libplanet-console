using System.Reflection;
using System.Text.RegularExpressions;

namespace LibplanetConsole.Consoles;

internal static class ProcessUtility
{
    private const string WorkspacePathVariableName = "LIBPLANET_CONSOLE_WORKSPACE_PATH";
    private const string NodePathVariableName = "LIBPLANET_CONSOLE_NODE_PATH";
    private const string ClientPathVariableName = "LIBPLANET_CONSOLE_CLIENT_PATH";
    private const string Framework = "net8.0";

#if DEBUG
    private const string Congiguration = "Debug";
#elif RELEASE
    private const string Congiguration = "Release";
#endif

    public static string WorkspacePath
    {
        get
        {
            var s = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
            if (Environment.GetEnvironmentVariable(WorkspacePathVariableName) is { } workspacePath)
            {
                if (Directory.Exists(workspacePath) != true)
                {
                    var message =
                        $"Directory '{workspacePath}' of environment variable " +
                        $"'{WorkspacePathVariableName}' does not exist.";
                    throw new InvalidOperationException(message);
                }

                return Regex.Replace(workspacePath, "[/\\\\]$", string.Empty);
            }
            else
            {
                return Environment.CurrentDirectory;
            }
        }
    }

    public static string NodePath
    {
        get
        {
            if (Environment.GetEnvironmentVariable(NodePathVariableName) is { } nodePath)
            {
                if (File.Exists(nodePath) != true)
                {
                    var message =
                        $"File '{nodePath}' of environment variable " +
                        $"'{NodePathVariableName}' does not exist.";
                    throw new FileNotFoundException(message);
                }

                return nodePath;
            }

            var actualNodePath = IsInProject ? NodePathInProject : NodePathInBin;
            if (File.Exists(actualNodePath) != true)
            {
                var message = $"File '{actualNodePath}' does not exist.";
                throw new FileNotFoundException(message);
            }

            return actualNodePath;
        }
    }

    public static string ClientPath
    {
        get
        {
            if (Environment.GetEnvironmentVariable(ClientPathVariableName) is { } clientPath)
            {
                if (File.Exists(clientPath) != true)
                {
                    var message =
                        $"File '{clientPath}' of environment variable " +
                        $"'{ClientPathVariableName}' does not exist.";
                    throw new FileNotFoundException(message);
                }

                return clientPath;
            }

            var actualClientPath = IsInProject ? ClientPathInProject : ClientPathInBin;
            if (File.Exists(actualClientPath) != true)
            {
                var message = $"File '{actualClientPath}' does not exist.";
                throw new FileNotFoundException(message);
            }

            return actualClientPath;
        }
    }

    public static string Extension => IsWindows() && IsArm64() ? ".exe" : ".dll";

    private static bool IsInProject
    {
        get
        {
            var location = Assembly.GetExecutingAssembly().Location ??
                throw new InvalidOperationException("Executing assembly location is not found.");
            var directory = Path.GetDirectoryName(location) ??
                throw new InvalidOperationException(
                    $"Directory of the executing assembly location '{location}' is not found.");
            var expectedDirectory = $"{WorkspacePath}/src/console/LibplanetConsole.ConsoleHost/" +
                                    $"bin/{Congiguration}/{Framework}";
            var d1 = Path.GetFullPath(expectedDirectory);
            var d2 = Path.GetFullPath(directory);
            return d1 == d2;
        }
    }

    private static string NodePathInProject
    {
        get
        {
            return Path.GetFullPath(
                $"{WorkspacePath}/src/node/LibplanetConsole.NodeHost/bin/{Congiguration}/" +
                $"{Framework}/libplanet-node{Extension}"
            );
        }
    }

    private static string NodePathInBin
        => Path.GetFullPath($"{WorkspacePath}/.bin/libplanet-node/libplanet-node{Extension}");

    private static string ClientPathInProject
    {
        get
        {
            return Path.GetFullPath(
                $"{WorkspacePath}/src/client/LibplanetConsole.ClientHost/bin/{Congiguration}/" +
                $"{Framework}/libplanet-client{Extension}"
            );
        }
    }

    private static string ClientPathInBin
        => Path.GetFullPath($"{WorkspacePath}/.bin/libplanet-client/libplanet-client{Extension}");

    public static string GetNodePath()
    {
        try
        {
            return NodePath;
        }
        catch (Exception e)
        {
            var message =
                $"Use 'src/node/LibplanetConsole.NodeHost/bin/{Congiguration}/{Framework}/" +
                $"libplanet-node{Extension}' by setting the directory path " +
                $"in environment variable '{WorkspacePathVariableName}', or " +
                $"set the path to the node executable DLL file directly in environment variable " +
                $"'{NodePathVariableName}'.";
            throw new InvalidOperationException(message, innerException: e);
        }
    }

    public static string GetClientPath()
    {
        try
        {
            return ClientPath;
        }
        catch (Exception e)
        {
            var message =
                $"Use 'src/client/LibplanetConsole.ClientHost/bin/{Congiguration}/" +
                $"{Framework}/libplanet-client{Extension}' by setting the directory path " +
                $"in environment variable '{WorkspacePathVariableName}', or " +
                $"set the path to the node executable DLL file directly in environment variable " +
                $"'{ClientPathVariableName}'.";
            throw new InvalidOperationException(message, innerException: e);
        }
    }

    public static bool IsWindows()
    {
        return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows);
    }

    public static bool IsArm64()
    {
        return System.Runtime.InteropServices.RuntimeInformation.OSArchitecture ==
               System.Runtime.InteropServices.Architecture.Arm64;
    }
}
