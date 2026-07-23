using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Genre
{
    [XmlText]
    [JsonPropertyName("value")]
    public required string Name { get; set; }

    [XmlAttribute("songCount")]
    [JsonPropertyName("songCount")]
    public required int SongCount { get; set; }

    [XmlAttribute("albumCount")]
    [JsonPropertyName("albumCount")]
    public required int AlbumCount { get; set; }
}