using System.Xml;
using TableCloth2.Models;
using TableCloth2.Services;
using TableCloth2.Shared.Services;
using TableCloth2.TableCloth.Contracts;

namespace TableCloth2.TableCloth.Services;

public sealed class WindowsSandboxComposer : ISandboxComposer
{
    public WindowsSandboxComposer(
        ProcessUtility processUtility,
        SessionService sessionService,
        KnownPathsService knownPathService)
    {
        _processUtility = processUtility;
        _sessionService = sessionService;
        _knownPathsService = knownPathService;
    }

    private readonly ProcessUtility _processUtility;
    private readonly SessionService _sessionService;
    private readonly KnownPathsService _knownPathsService;

    public XmlDocument ComposeSandboxDocument(SettingsModel settingsModel)
    {
        var doc = new XmlDocument();

        var configurationElem = doc.CreateElement("Configuration");
        {
            var currentProcessFilePath = _processUtility.GetExecutableFile();
            var execDirectoryPath = currentProcessFilePath.DirectoryName ??
                throw new Exception($"Cannot obtain directory path of the current process file path: {currentProcessFilePath}");
            var execDirectoryName = Path.GetFileName(execDirectoryPath) ??
                throw new Exception($"Cannot obtain folder name of the directory path: {execDirectoryPath}");

            var mappedFoldersElem = doc.CreateElement("MappedFolders");
            {
                var list = new Dictionary<string, bool>
                {
                    { execDirectoryPath, true },
                    { _knownPathsService.EnsureLocalLowNPKIDirectoryExists().FullName, true },
                    { _knownPathsService.EnsureTableClothSettingsDirectoryExists().FullName, true },
                };

                if (settingsModel.EnableFolderMount)
                {
                    foreach (var eachFolder in settingsModel.FolderMountList)
                        list.Add(eachFolder, false);
                }

                foreach (var eachPair in list)
                {
                    var mappedFolderElem = doc.CreateElement("MappedFolder");
                    {
                        var hostFolderElem = doc.CreateElement("HostFolder");
                        hostFolderElem.InnerText = eachPair.Key;
                        mappedFolderElem.AppendChild(hostFolderElem);

                        var readOnlyElem = doc.CreateElement("ReadOnly");
                        readOnlyElem.InnerText = eachPair.Value.ToString().ToLowerInvariant();
                        mappedFolderElem.AppendChild(readOnlyElem);
                    }
                    mappedFoldersElem.AppendChild(mappedFolderElem);
                }
            }
            configurationElem.AppendChild(mappedFoldersElem);

            var logonCommandElem = doc.CreateElement("LogonCommand");
            {
                var commandElem = doc.CreateElement("Command");
                commandElem.InnerText = @$"C:\Users\WDAGUtilityAccount\Desktop\{execDirectoryName}\TableCloth2.exe --mode=Spork --privileged=true";
                logonCommandElem.AppendChild(commandElem);
            }
            configurationElem.AppendChild(logonCommandElem);

            var vgpuElem = doc.CreateElement("vGPU");
            vgpuElem.InnerText = settingsModel.EnableVirtualizedGpu ? "Enable" : "Disable";
            configurationElem.AppendChild(vgpuElem);

            //var networkingElem = doc.CreateElement("Networking");
            //networkingElem.InnerText = "Enable";
            //configurationElem.AppendChild(networkingElem);

            var audioInputElem = doc.CreateElement("AudioInput");
            audioInputElem.InnerText = settingsModel.EnableAudioInput ? "Enable" : "Disable";
            configurationElem.AppendChild(audioInputElem);

            var videoInputElem = doc.CreateElement("VideoInput");
            videoInputElem.InnerText = settingsModel.EnableVideoInput ? "Enable" : "Disable";
            configurationElem.AppendChild(videoInputElem);

            //var protectedClientElem = doc.CreateElement("ProtectedClient");
            //protectedClientElem.InnerText = "Disable";
            //configurationElem.AppendChild(protectedClientElem);

            var printerRedirectionElem = doc.CreateElement("PrinterRedirection");
            printerRedirectionElem.InnerText = settingsModel.EnablePrinterRedirection ? "Enable" : "Disable";
            configurationElem.AppendChild(printerRedirectionElem);

            //var clipboardRedirectionElem = doc.CreateElement("ClipboardRedirection");
            //clipboardRedirectionElem.InnerText = "Enable";
            //configurationElem.AppendChild(clipboardRedirectionElem);

            //var memoryInMBElem = doc.CreateElement("MemoryInMB");
            //memoryInMBElem.InnerText = (2048).ToString();
            //configurationElem.AppendChild(memoryInMBElem);
        }
        doc.AppendChild(configurationElem);

        return doc;
    }

    public FileInfo CreateSandboxSpec(SettingsModel settingsModel)
    {
        var directory = _sessionService.CreateSessionDirectory();
        var sandboxSpec = this.ComposeSandboxDocument(settingsModel);
        var sandboxSpecFile = new FileInfo(directory.Combine("sandbox.wsb"));

        using (var fileStream = sandboxSpecFile.Open(FileMode.Create))
        {
            sandboxSpec.Save(fileStream);
        }

        return sandboxSpecFile;
    }
}
