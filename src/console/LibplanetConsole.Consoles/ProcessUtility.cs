using System.Reflection;
using System.Runtime.InteropServices;
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

    public static string PublishExtension => IsWindows() ? ".exe" : ".dll";

    public static string DotnetPath
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                return @"C:\Program Files\dotnet\dotnet.exe";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) == true)
            {
                return "/usr/local/share/dotnet/dotnet";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) == true)
            {
                return "/usr/bin/dotnet";
            }

            throw new NotSupportedException("Unsupported OS platform.");
        }
    }

    private static bool IsInProject
    {
        get
        {
            var location = Assembly.GetExecutingAssembly().Location ??
                throw new InvalidOperationException("Executing assembly location is not found.");
            var directory = Path.GetDirectoryName(location) ??
                throw new InvalidOperationException(
                    $"Directory of the executing assembly location '{location}' is not found.");
            var expectedDirectory = $"{WorkspacePath}/src/console/" +
                                    $"LibplanetConsole.Consoles.Executable/" +
                                    $"bin/{Congiguration}/{Framework}";
            var d1 = Path.GetFullPath(expectedDirectory);
            var d2 = Path.GetFullPath(directory);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(d1, d2);
            }

            return d1 == d2;
        }
    }

    private static string NodePathInProject
    {
        get
        {
            return Path.GetFullPath(
                $"{WorkspacePath}/src/node/LibplanetConsole.Nodes.Executable/bin/{Congiguration}/" +
                $"{Framework}/libplanet-node.dll"
            );
        }
    }

    private static string NodePathInBin
    {
        get
        {
            if (Environment.ProcessPath is { } processPath)
            {
                var directoryName = Path.GetDirectoryName(processPath) ??
                    throw new InvalidOperationException(
                        $"Directory of the process path '{processPath}' is not found.");
                var extension = Path.GetExtension(processPath) ??
                    throw new InvalidOperationException(
                        $"Extension of the process path '{processPath}' is not found.");
                return Path.Combine(directoryName, $"libplanet-node{extension}");
            }

            throw new InvalidOperationException("Environment.ProcessPath is not found.");
        }
    }

    private static string ClientPathInProject
    {
        get
        {
            return Path.GetFullPath(
                $"{WorkspacePath}/src/client/LibplanetConsole.Clients.Executable/bin/" +
                $"{Congiguration}/{Framework}/libplanet-client.dll"
            );
        }
    }

    private static string ClientPathInBin
    {
        get
        {
            if (Environment.ProcessPath is { } processPath)
            {
                var directoryName = Path.GetDirectoryName(processPath) ??
                    throw new InvalidOperationException(
                        $"Directory of the process path '{processPath}' is not found.");
                var extension = Path.GetExtension(processPath) ??
                    throw new InvalidOperationException(
                        $"Extension of the process path '{processPath}' is not found.");
                return Path.Combine(directoryName, $"libplanet-client{extension}");
            }

            throw new InvalidOperationException("Environment.ProcessPath is not found.");
        }
    }

    public static string GetAssemblyLocation(Assembly assembly)
    {
        if (assembly.Location is { } assemblyLocation)
        {
            return assemblyLocation;
        }

        return Path.Combine(AppContext.BaseDirectory, assembly.GetName().Name!);
    }

    public static string GetNodePath()
    {
        try
        {
            return NodePath;
        }
        catch (Exception e)
        {
            var message =
                $"Use 'src/node/LibplanetConsole.Nodes.Executable/bin/{Congiguration}/" +
                $"{Framework}/libplanet-node{PublishExtension}' by setting the directory path " +
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
                $"Use 'src/client/LibplanetConsole.Clients.Executable/bin/{Congiguration}/" +
                $"{Framework}/libplanet-client{PublishExtension}' by setting the directory path " +
                $"in environment variable '{WorkspacePathVariableName}', or " +
                $"set the path to the node executable DLL file directly in environment variable " +
                $"'{ClientPathVariableName}'.";
            throw new InvalidOperationException(message, innerException: e);
        }
    }

    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    public static bool IsArm64()
    {
        return RuntimeInformation.OSArchitecture == Architecture.Arm64;
    }

    public static bool IsDotnetRuntime()
    {
        return Environment.ProcessPath == DotnetPath;
    }
}
