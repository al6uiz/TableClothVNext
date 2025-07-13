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
        LauncherMainWindowViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var root = new XElement("Configuration");
        root.Add(new XElement("vGPU", "Disable"));
        root.Add(new XElement("Networking", "Enable"));
        root.Add(new XElement("AudioInput", viewModel.UseMicrophone ? "Enable" : "Disable"));
        root.Add(new XElement("VideoInput", viewModel.UseWebCamera ? "Enable" : "Disable"));
        root.Add(new XElement("ProtectedClient", "Disable"));
        root.Add(new XElement("PrinterRedirection", viewModel.SharePrinters ? "Enable" : "Disable"));
        root.Add(new XElement("ClipboardRedirection", "Enable"));

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
