using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class NameEntity
{
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}