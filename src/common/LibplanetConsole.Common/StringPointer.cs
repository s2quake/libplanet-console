using System.Runtime.InteropServices;
using System.Security;

namespace LibplanetConsole.Common;

internal sealed class StringPointer(SecureString secureString) : IDisposable
{
    private readonly IntPtr _ptr
        = Marshal.SecureStringToGlobalAllocUnicode(secureString);

    public string GetString() => Marshal.PtrToStringUni(_ptr) switch
    {
        { } @string => @string,
        _ => throw new InvalidOperationException("Failed to get a string from the pointer."),
    };

    public void Dispose() => Marshal.ZeroFreeGlobalAllocUnicode(_ptr);
}
