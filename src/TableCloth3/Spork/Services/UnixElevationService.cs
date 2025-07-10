using System.Diagnostics;
using System.Runtime.InteropServices;
using TableCloth3.Spork.Contracts;

namespace TableCloth3.Spork.Services;

public class UnixElevationService : IElevationService
{
    public bool IsElevated()
    {
        return GetUidSafe() == 0;
    }

    public void RestartAsElevated(string[] args)
    {
        var exeName = GetCurrentExecutablePath();
        if (string.IsNullOrEmpty(exeName))
            throw new InvalidOperationException("실행 파일 경로를 찾을 수 없습니다. 수동으로 root 권한으로 실행하십시오.");

        if (!File.Exists("/bin/sh"))
            throw new InvalidOperationException("/bin/sh가 시스템에 없습니다. 수동으로 root 권한으로 실행하십시오.");

        string? sudoPath = FindExecutableInPath("sudo");
        if (string.IsNullOrEmpty(sudoPath))
            throw new InvalidOperationException("sudo를 찾을 수 없습니다. 수동으로 root 권한으로 실행하십시오.");

        string argList = args != null && args.Length > 0
            ? string.Join(" ", Array.ConvertAll(args, arg => $"\"{arg}\""))
            : "";
        string shellArgs = $"-c \"exec sudo '{exeName}' {argList}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/sh",
            Arguments = shellArgs,
            UseShellExecute = false
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("sudo를 통한 권한 상승 실행 실패: " + ex.Message, ex);
        }

        Environment.Exit(0);
    }

    private int GetUidSafe()
    {
        try { return getuid(); }
        catch { return -1; }
    }

    [DllImport("libc")]
    private static extern int getuid();

    private string? GetCurrentExecutablePath()
    {
        try { return Environment.ProcessPath; }
        catch { return null; }
    }

    private string? FindExecutableInPath(string exeName)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathEnv))
            return null;

        foreach (var dir in pathEnv.Split(Path.PathSeparator))
        {
            try
            {
                var candidate = Path.Combine(dir.Trim(), exeName);
                if (File.Exists(candidate))
                    return candidate;
            }
            catch { }
        }
        return null;
    }
}
