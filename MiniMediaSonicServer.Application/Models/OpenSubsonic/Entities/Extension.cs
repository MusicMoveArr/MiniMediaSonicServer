using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Extension
{
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [XmlElement("versions")]
    [JsonPropertyName("versions")]
    public List<int> Versions { get; set; }
}