using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using TableCloth3.Launcher.ViewModels;
using TableCloth3.Shared.Services;

namespace TableCloth3.Launcher.Services;

public sealed class WindowsSandboxComposer
{
    public WindowsSandboxComposer(
        LocationService locationService)
        : base()
    {
        _locationService = locationService;
    }

    private readonly LocationService _locationService = default!;

    public async Task<string> GenerateWindowsSandboxProfileAsync(
        LauncherMainWindowViewModel launcherViewModel,
        FolderManageWindowViewModel folderViewModel,
        List<string> warnings,
        CancellationToken cancellationToken = default)
    {
        var root = new XElement("Configuration");
        root.Add(new XElement("vGPU", "Disable"));
        root.Add(new XElement("Networking", "Enable"));
        root.Add(new XElement("AudioInput", launcherViewModel.UseMicrophone ? "Enable" : "Disable"));
        root.Add(new XElement("VideoInput", launcherViewModel.UseWebCamera ? "Enable" : "Disable"));
        root.Add(new XElement("ProtectedClient", "Disable"));
        root.Add(new XElement("PrinterRedirection", launcherViewModel.SharePrinters ? "Enable" : "Disable"));
        root.Add(new XElement("ClipboardRedirection", "Enable"));
        root.Add(new XElement("MemoryInMB", 2048));

        var mappedFoldersElem = new XElement("MappedFolders");
        var aliasList = new List<string>();

        if (launcherViewModel.MountNpkiFolders)
        {
            var targetPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "AppData", "LocalLow", "NPKI");

            if (Directory.Exists(targetPath))
            {
                var alias = Path.GetFileName(targetPath);
                if (!aliasList.Contains(alias))
                {
                    var eachMappedFolderElem = new XElement("MappedFolder");

                    var hostFolderElem = new XElement("HostFolder", targetPath);
                    eachMappedFolderElem.Add(hostFolderElem);

                    mappedFoldersElem.Add(eachMappedFolderElem);
                    aliasList.Add(alias);
                }
                else
                    warnings.Add($"Selected directory '{targetPath}' cannot be mounted due to duplicated name.");
            }
            else
                warnings.Add($"Selected directory '{targetPath}' does not exists.");
        }

        if (launcherViewModel.MountSpecificFolders)
        {
            foreach (var eachFolder in folderViewModel.Folders)
            {
                // TODO: 마운트하지 못한 폴더 (존재하지 않거나 이름이 겹치는 폴더)에 대한 경고 메시지 표시 추가
                var targetPath = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(
                        eachFolder));

                if (!Directory.Exists(targetPath))
                {
                    warnings.Add($"Selected directory '{targetPath}' does not exists.");
                    continue;
                }

                var alias = Path.GetFileName(targetPath);
                if (aliasList.Contains(alias))
                {
                    warnings.Add($"Selected directory '{targetPath}' cannot be mounted due to duplicated name.");
                    continue;
                }

                var eachMappedFolderElem = new XElement("MappedFolder");

                var hostFolderElem = new XElement("HostFolder", targetPath);
                eachMappedFolderElem.Add(hostFolderElem);

                mappedFoldersElem.Add(eachMappedFolderElem);
                aliasList.Add(alias);
            }
        }

        if (aliasList.Any())
            root.Add(mappedFoldersElem);

        var doc = new XDocument(root);
        _locationService.EnsureAppDataDirectoryCreated();
        using var fileStream = File.Open(_locationService.WindowsSandboxProfilePath, FileMode.Create);

        using var xw = XmlWriter.Create(fileStream, new XmlWriterSettings()
        {
            Indent = true,
            OmitXmlDeclaration = true,
            Async = true,
        });

        await doc.SaveAsync(xw, cancellationToken).ConfigureAwait(false);
        return _locationService.WindowsSandboxProfilePath;
    }
}
