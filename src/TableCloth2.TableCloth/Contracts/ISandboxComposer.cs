using System.Xml;
using TableCloth2.Models;

namespace TableCloth2.TableCloth.Contracts
{
    public interface ISandboxComposer
    {
        XmlDocument ComposeSandboxDocument(SettingsModel settingsModel);
        FileInfo CreateSandboxSpec(SettingsModel settingsModel);
    }
}