using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Genre
{
    [XmlText]
    [JsonPropertyName("value")]
    public string Name { get; set; }

    [XmlAttribute("songCount")]
    [JsonPropertyName("songCount")]
    public int SongCount { get; set; }

    [XmlAttribute("albumCount")]
    [JsonPropertyName("albumCount")]
    public int AlbumCount { get; set; }
}