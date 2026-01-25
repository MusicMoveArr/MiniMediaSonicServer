using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class ArtistID3
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [XmlAttribute("coverArt")]
    [JsonPropertyName("coverArt")]
    public string CoverArt { get; set; }
    
    [XmlAttribute("artistImageUrl")]
    [JsonPropertyName("artistImageUrl")]
    public string ArtistImageUrl { get; set; }
    
    [XmlAttribute("albumCount")]
    [JsonPropertyName("albumCount")]
    public int AlbumCount { get; set; }
    
    [XmlElement("starred")]
    [JsonPropertyName("starred")]
    public DateTime? Starred { get; set; }
    
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public List<AlbumID3> Albums { get; set; }
    
    [XmlElement("musicBrainzId")]
    [JsonPropertyName("musicBrainzId")]
    public Guid? MusicBrainzId { get; set; }
    
    [XmlAttribute("sortName")]
    [JsonPropertyName("sortName")]
    public string SortName { get; set; }

    [XmlElement("roles")]
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = ["artist", "albumartist"];
}