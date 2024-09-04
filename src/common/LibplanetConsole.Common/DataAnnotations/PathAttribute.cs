using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PathAttribute : ValidationAttribute
{
    internal const string CurrentDirectory = nameof(CurrentDirectory);

    public bool IsDirectory { get; set; }

    public bool MustExist { get; set; }

    public bool IsAbsoluteOrRelative { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string path)
        {
            if (IsAbsoluteOrRelative is false && Path.IsPathRooted(path) is false)
            {
                var currentDirectory = GetCurrentDirectory(validationContext);
                var objectType = validationContext.ObjectType;
                var absolutePath = Path.GetFullPath(Path.Combine(currentDirectory, path));
                var memberName = validationContext.MemberName;
                var instance = validationContext.ObjectInstance;
                if (memberName is null)
                {
                    return new ValidationResult("MemberName is null.");
                }

                var propertyInfo = objectType.GetProperty(memberName);
                if (propertyInfo is null)
                {
                    return new ValidationResult("Property not found.");
                }

                if (propertyInfo.CanWrite is false)
                {
                    return new ValidationResult("Property is read-only.");
                }

                propertyInfo.SetValue(instance, absolutePath);
                path = absolutePath;
            }

            if (IsDirectory is true)
            {
                if (File.Exists(path) is true)
                {
                    return new ValidationResult("File exists but directory is required.");
                }
            }
            else
            {
                if (Directory.Exists(path) is true)
                {
                    return new ValidationResult("Directory exists but file is required.");
                }
            }

            if (MustExist is true)
            {
                if (IsDirectory is true)
                {
                    if (Directory.Exists(path) is false)
                    {
                        return new ValidationResult("Directory does not exist.");
                    }
                }
                else
                {
                    if (File.Exists(path) is false)
                    {
                        return new ValidationResult("File does not exist.");
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
