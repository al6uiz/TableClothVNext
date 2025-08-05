using System.Runtime.InteropServices;

namespace TableCloth3.Shared;

internal static class SafeNativeMethods
{
    private static readonly Guid FOLDERID_LocalAppDataLow =
        new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");

    [DllImport("shell32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
        [MarshalAs(UnmanagedType.U4)] int dwFlags,
        nint hToken,
        out nint ppszPath);

    public static string GetLocalAppDataLowFolder(Environment.SpecialFolderOption options = default)
    {
        var hr = SHGetKnownFolderPath(
            FOLDERID_LocalAppDataLow, (int)options, default, out var outPath);

        if (hr != 0)
            throw new ExternalException("Failed to get known folder path", hr);

        var path = Marshal.PtrToStringUni(outPath);
        Marshal.FreeCoTaskMem(outPath);

        if (string.IsNullOrWhiteSpace(path))
            throw new Exception($"Cannot retrieve known folder path for AppData\\LocalLow directory.");

        return path;
    }
}
