using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PathAttribute : ValidationAttribute
{
    internal const string CurrentDirectory = nameof(CurrentDirectory);

    /// <summary>
    /// Gets or sets the path type.
    /// </summary>
    public PathType Type { get; set; }

    /// <summary>
    /// Gets or sets the path exists type.
    /// </summary>
    public PathExistsType ExistsType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the path is absolute.
    /// </summary>
    public bool IsAbsolute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the path can be empty.
    /// </summary>
    public bool AllowEmpty { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is "" && AllowEmpty is true)
        {
            return ValidationResult.Success;
        }

        if (value is string path)
        {
            var memberName = validationContext.MemberName;
            var objectType = validationContext.ObjectType;
            if (memberName is null)
            {
                return new ValidationResult("MemberName is null.");
            }

            var memberNames = new string[] { memberName };
            var propertyInfo = objectType.GetProperty(memberName);
            if (propertyInfo is null)
            {
                return new ValidationResult("Property not found.");
            }

            if (propertyInfo.CanWrite is false)
            {
                return new ValidationResult("Property is read-only.");
            }

            if (path == string.Empty)
            {
                return new ValidationResult(
                    $"Property '{validationContext.MemberName}' cannot be empty.", memberNames);
            }

            if (IsAbsolute is false && Path.IsPathRooted(path) is false)
            {
                var currentDirectory = GetCurrentDirectory(validationContext);
                var absolutePath = Path.GetFullPath(Path.Combine(currentDirectory, path));
                var instance = validationContext.ObjectInstance;
                propertyInfo.SetValue(instance, absolutePath);
                path = absolutePath;
            }

            if (Type is PathType.Directory && File.Exists(path) is true)
            {
                var message = $"Path '{path}' of property '{memberName}' " +
                              $"must be a directory, but it is a file.";
                return new ValidationResult(message, memberNames);
            }
            else if (Type is PathType.File && Directory.Exists(path) is true)
            {
                var message = $"Path '{path}' of property '{memberName}' " +
                              $"must be a file, but it is a directory.";
                return new ValidationResult(message, memberNames);
            }

            if (ExistsType is PathExistsType.Exist)
            {
                if (Type is PathType.Directory)
                {
                    if (Directory.Exists(path) is false)
                    {
                        var message = $"Directory '{path}' of property " +
                                      $"'{memberName}' does not exist.";
                        return new ValidationResult(message, memberNames);
                    }
                }
                else if (Type is PathType.File)
                {
                    if (File.Exists(path) is false)
                    {
                        var message = $"File '{path}' of property " +
                                      $"'{memberName}' does not exist.";
                        return new ValidationResult(message, memberNames);
                    }
                }
                else
                {
                    if (Directory.Exists(path) is false && File.Exists(path) is false)
                    {
                        var message = $"Path '{path}' of property '{memberName}' " +
                                    "does not exist.";
                        return new ValidationResult(message, memberNames);
                    }
                }
            }
            else if (ExistsType is PathExistsType.NotExist)
            {
                if (Type is PathType.Directory)
                {
                    if (Directory.Exists(path) is true)
                    {
                        var message = $"Directory '{path}' of property " +
                                      $"'{memberName}' already exists.";
                        return new ValidationResult(message, memberNames);
                    }
                }
                else if (Type is PathType.File)
                {
                    if (File.Exists(path) is true)
                    {
                        var message = $"File '{path}' of property " +
                                      $"'{memberName}' already exists.";
                        return new ValidationResult(message, memberNames);
                    }
                }
                else
                {
                    if (Directory.Exists(path) is true || File.Exists(path) is true)
                    {
                        var message = $"Path '{path}' of property '{memberName}' " +
                                    "already exists.";
                        return new ValidationResult(message, memberNames);
                    }
                }
            }
            else if (ExistsType is PathExistsType.NotExistOrEmpty)
            {
                if (Type is PathType.Directory)
                {
                    if (Directory.Exists(path) is true && Directory.GetFiles(path).Length > 0)
                    {
                        var message = $"Directory '{path}' of property " +
                                      $"'{memberName}' already exists and is not empty.";
                        return new ValidationResult(message, memberNames);
                    }
                }
                else if (Type is PathType.File)
                {
                    if (File.Exists(path) is true && new FileInfo(path).Length > 0)
                    {
                        var message = $"File '{path}' of property " +
                                      $"'{memberName}' already exists and is not empty.";
                        return new ValidationResult(message, memberNames);
                    }
                }
                else
                {
                    if ((Directory.Exists(path) is true && Directory.GetFiles(path).Length > 0) ||
                        (File.Exists(path) is true && new FileInfo(path).Length > 0))
                    {
                        var message = $"Path '{path}' of property '{memberName}' " +
                                    "already exists and is not empty.";
                        return new ValidationResult(message, memberNames);
                    }
                }
            }
        }

        return ValidationResult.Success;
    }

    private static string GetCurrentDirectory(ValidationContext validationContext)
    {
        if (validationContext.Items.TryGetValue(CurrentDirectory, out var value) is false)
        {
            return Directory.GetCurrentDirectory();
        }

        return value as string ?? Directory.GetCurrentDirectory();
    }
}
