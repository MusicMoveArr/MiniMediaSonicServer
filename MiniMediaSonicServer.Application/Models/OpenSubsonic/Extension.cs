using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic;

public class Extension
{
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [XmlAttribute("versions")]
    [JsonIgnore]
    public string VersionsXml { get; set; } = "";

    [XmlIgnore]
    [JsonPropertyName("versions")]
    public List<int> Versions { get; set; } = new();
}