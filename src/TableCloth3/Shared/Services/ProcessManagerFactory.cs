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

        return Process.Start(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"),
            $"/c \"{fileName}\" {arguments}");
    }
}
