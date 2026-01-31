using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class StarredResponse
{
    [XmlElement("artist")]
    [JsonPropertyName("artist")]
    public List<ArtistID3> Artists { get; set; } = new();
    
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public List<AlbumID3> Albums { get; set; } = new();
    
    [XmlElement("song")]
    [JsonPropertyName("song")]
    public List<TrackID3> Tracks { get; set; } = new();
}