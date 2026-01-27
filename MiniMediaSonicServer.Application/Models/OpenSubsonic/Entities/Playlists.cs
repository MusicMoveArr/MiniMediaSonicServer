using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Playlists
{
    [XmlElement("playlist")]
    [JsonPropertyName("playlist")]
    public List<Playlist> Playlist { get; set; } = new();
}