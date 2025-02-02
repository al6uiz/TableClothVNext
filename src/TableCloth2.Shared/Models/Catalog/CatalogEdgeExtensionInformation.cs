using System.Xml;
using System.Xml.Serialization;

namespace TableCloth2.Shared.Models.Catalog;

[Serializable, XmlType]
public sealed class CatalogEdgeExtensionInformation
{
    [XmlAttribute("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("CrxUrl")]
    public string CrxUrl { get; set; } = string.Empty;

    [XmlAttribute("ExtensionId")]
    public string ExtensionId { get; set; } = string.Empty;
}
