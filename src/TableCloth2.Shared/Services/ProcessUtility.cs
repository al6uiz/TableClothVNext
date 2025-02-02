using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Security.Principal;

namespace TableCloth2.Shared.Services;

public sealed class ProcessUtility
{
    public ProcessUtility(
        Configurations configurations,
        IServiceProvider serviceProvider)
    {
        this._configurations = configurations;
        this._serviceProvider = serviceProvider;
        this._lifetime = _serviceProvider.GetService<IHostApplicationLifetime>();
        this._arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
    }

    private readonly Configurations _configurations;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime? _lifetime;
    private readonly string[] _arguments;

    public bool HasPrivilegeRequested => _configurations.Privileged;

    public IReadOnlyList<string> Arguments => _arguments;

    public bool HasAdministratorRole()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public FileInfo GetExecutableFile()
    {
        var fileName = Process.GetCurrentProcess().MainModule?.FileName;

        if (string.IsNullOrWhiteSpace(fileName))
            throw new Exception("Cannot retrieve current process file name.");

        return new FileInfo(fileName);
    }

    public DirectoryInfo GetExecutableFileDirectory()
    {
        return GetExecutableFile().Directory ??
            throw new Exception("Cannot retrieve current process file directory.");
    }

    public void Restart(bool withPrivileged)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = GetExecutableFile().FullName,
            Arguments = string.Join(" ", _arguments),
        };

        if (withPrivileged)
        {
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = true;
        }
        else
        {
            startInfo.Verb = string.Empty;
            startInfo.UseShellExecute = false;
        }

        Process.Start(startInfo);

        if (_lifetime != null)
            _lifetime.StopApplication();
        else
            Environment.Exit(0);
        return;
    }
}
