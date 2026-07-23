using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class SearchResult3
{
    [XmlElement("artist")]
    [JsonPropertyName("artist")]
    public required List<ArtistID3> Artists { get; set; } = [];
    
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public required List<AlbumID3> Albums { get; set; } = [];
    
    [XmlElement("song")]
    [JsonPropertyName("song")]
    public required List<TrackID3> Tracks { get; set; } = [];
}