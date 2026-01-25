using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class AlbumID3
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [XmlAttribute("version")]
    [JsonPropertyName("version")]
    public string Version { get; set; }
    
    [XmlAttribute("artist")]
    [JsonPropertyName("artist")]
    public string Artist { get; set; }
    
    [XmlElement("artistId")]
    [JsonPropertyName("artistId")]
    public Guid ArtistId { get; set; }
    
    [XmlAttribute("coverArt")]
    [JsonPropertyName("coverArt")]
    public string CoverArt { get; set; }
    
    [XmlAttribute("year")]
    [JsonPropertyName("year")]
    public int Year { get; set; }
    
    [XmlElement("starred")]
    [JsonPropertyName("starred")]
    public DateTime? Starred { get; set; }
    
    [XmlAttribute("duration")]
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [XmlAttribute("playCount")]
    [JsonPropertyName("playCount")]
    public long PlayCount { get; set; }
    
    [XmlAttribute("genre")]
    [JsonPropertyName("genre")]
    public string Genre { get; set; }
    
    [XmlAttribute("created")]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
    
    [XmlAttribute("songCount")]
    [JsonPropertyName("songCount")]
    public int SongCount { get; set; }
    
    [XmlAttribute("played")]
    [JsonPropertyName("played")]
    public DateTime Played { get; set; }
    
    [XmlAttribute("userRating")]
    [JsonPropertyName("userRating")]
    public int UserRating { get; set; }
    
    [XmlElement("musicBrainzId")]
    [JsonPropertyName("musicBrainzId")]
    public Guid? MusicBrainzId { get; set; }
    
    [XmlAttribute("displayArtist")]
    [JsonPropertyName("displayArtist")]
    public string DisplayArtist { get; set; }
    
    [XmlAttribute("sortName")]
    [JsonPropertyName("sortName")]
    public string SortName { get; set; }
    
    [XmlAttribute("isCompilation")]
    [JsonPropertyName("isCompilation")]
    public bool IsCompilation { get; set; }
    
    [XmlAttribute("explicitStatus")]
    [JsonPropertyName("explicitStatus")]
    public string ExplicitStatus { get; set; }
    
    [XmlElement("artists")]
    [JsonPropertyName("artists")]
    public List<NameIdEntity> Artists { get; set; }

    [XmlElement("song")]
    [JsonPropertyName("song")]
    public List<TrackID3> Song { get; set; }

}