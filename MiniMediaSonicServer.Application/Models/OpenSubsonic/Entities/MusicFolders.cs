using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class MusicFolders
{
    [XmlElement("musicFolder")]
    [JsonPropertyName("musicFolder")]
    public List<MusicFolder> MusicFolder { get; set; } = new();
}