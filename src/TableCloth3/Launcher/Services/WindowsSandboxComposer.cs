using System.Diagnostics;
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

    private KeyValuePair<string, XElement>? CreateHostFolderMappingElement(string hostFolderPath, string? sandboxFolder = default, bool? readOnly = default)
    {
        if (!Directory.Exists(hostFolderPath))
            return null;

        var mappedFolderElem = new XElement("MappedFolder");
        var hostFolderElem = new XElement("HostFolder", hostFolderPath);
        mappedFolderElem.Add(hostFolderElem);

        if (!string.IsNullOrWhiteSpace(sandboxFolder))
        {
            var sandboxFolderElem = new XElement("SandboxFolder", sandboxFolder);
            mappedFolderElem.Add(sandboxFolderElem);
        }

        if (readOnly.HasValue)
        {
            var readOnlyElem = new XElement("ReadOnly", readOnly.Value ? "true" : "false");
            mappedFolderElem.Add(readOnlyElem);
        }

        if (string.IsNullOrWhiteSpace(sandboxFolder))
        {
            var alias = Path.GetFileName(hostFolderPath.Trim(Path.DirectorySeparatorChar));
            sandboxFolder = $"C:\\Users\\WDAGUtilityAccount\\Desktop\\{alias}";
        }

        return new(sandboxFolder, mappedFolderElem);
    }

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
        var foldersToMount = new Dictionary<string, XElement>();

        var processFilePath = Process.GetCurrentProcess().MainModule?.FileName;
        if (string.IsNullOrWhiteSpace(processFilePath))
            throw new Exception("Cannot determine application executable file path.");
        var thisDirectory = Path.GetDirectoryName(processFilePath);
        if (string.IsNullOrWhiteSpace(thisDirectory))
            throw new Exception("Cannot determine application directory path.");
        var thisFolder = CreateHostFolderMappingElement(thisDirectory);
        if (thisFolder == null)
            throw new Exception($"Cannot create host folder mapping element for '{thisDirectory}'.");

        var launcherAppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Launcher");
        var launcherAppDataFolderElem = CreateHostFolderMappingElement(launcherAppDataFolder);
        if (launcherAppDataFolderElem.HasValue)
            foldersToMount.Add(launcherAppDataFolderElem.Value.Key, launcherAppDataFolderElem.Value.Value);
        else
            warnings.Add($"Selected directory '{launcherAppDataFolder}' does not exists.");

        foldersToMount.Add(thisFolder.Value.Key, thisFolder.Value.Value);

        var logonCommandElem = new XElement("LogonCommand");
        var commandElem = new XElement("Command");
        commandElem.Value = $"{thisFolder.Value.Key}\\{Path.GetFileName(processFilePath)} --mode=Spork --sporkMode=Embedded";
        logonCommandElem.Add(commandElem);
        root.Add(logonCommandElem);

        if (launcherViewModel.MountNpkiFolders)
        {
            var npkiPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "AppData", "LocalLow", "NPKI");
            var npkiFolder = CreateHostFolderMappingElement(npkiPath);

            if (npkiFolder.HasValue)
                foldersToMount.Add(npkiFolder.Value.Key, npkiFolder.Value.Value);
            else
                warnings.Add($"Selected directory '{npkiPath}' does not exists.");
        }

        if (launcherViewModel.MountSpecificFolders)
        {
            foreach (var eachFolder in folderViewModel.Folders)
            {
                var targetPath = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(eachFolder));
                var targetItem = CreateHostFolderMappingElement(targetPath);

                if (targetItem.HasValue)
                    foldersToMount.Add(targetItem.Value.Key, targetItem.Value.Value);
                else
                    warnings.Add($"Selected directory '{targetPath}' does not exists.");
            }
        }

        if (foldersToMount.Any())
        {
            foreach (var eachMountPoint in foldersToMount)
                mappedFoldersElem.Add(eachMountPoint.Value);
        }

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
