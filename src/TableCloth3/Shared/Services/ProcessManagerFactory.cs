using System.Diagnostics;
using System.Threading;

namespace TableCloth3.Shared.Services;

public sealed class ProcessManagerFactory
{
    public ProcessManager Create()
        => new ProcessManager();

    public Process? RunThroughCmdShell(string fileName, string arguments = "")
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            throw new PlatformNotSupportedException("This method is only supported on Windows.");

        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"),
            Arguments = $"/c \"{fileName}\" {arguments}",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return Process.Start(startInfo);
    }
}
