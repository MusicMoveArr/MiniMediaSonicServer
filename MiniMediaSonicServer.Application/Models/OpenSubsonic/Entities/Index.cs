using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Index
{
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [XmlElement("artist")]
    [JsonPropertyName("artist")]
    public List<ArtistID3> Artist { get; set; } = new();
}