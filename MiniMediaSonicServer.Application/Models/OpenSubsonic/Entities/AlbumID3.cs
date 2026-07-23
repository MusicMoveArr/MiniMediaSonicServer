using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class AlbumID3
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }
    
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [XmlAttribute("version")]
    [JsonPropertyName("version")]
    public required string Version { get; init; }
    
    [XmlAttribute("artist")]
    [JsonPropertyName("artist")]
    public required string Artist { get; init; }
    
    [XmlElement("artistId")]
    [JsonPropertyName("artistId")]
    public required Guid ArtistId { get; init; }
    
    [XmlAttribute("coverArt")]
    [JsonPropertyName("coverArt")]
    public required string CoverArt { get; init; }
    
    [XmlElement("year")]
    [JsonPropertyName("year")]
    public int? Year { get; init; }
    
    [XmlElement("starred")]
    [JsonPropertyName("starred")]
    public DateTime? Starred { get; init; }
    
    [XmlAttribute("duration")]
    [JsonPropertyName("duration")]
    public required int Duration { get; set; }
    
    [XmlAttribute("playCount")]
    [JsonPropertyName("playCount")]
    public required long PlayCount { get; set; }
    
    [XmlAttribute("genre")]
    [JsonPropertyName("genre")]
    public required string Genre { get; init; }
    
    [XmlAttribute("created")]
    [JsonPropertyName("created")]
    public required DateTime Created { get; init; }
    
    [XmlAttribute("songCount")]
    [JsonPropertyName("songCount")]
    public required int SongCount { get; set; }
    
    [XmlElement("played")]
    [JsonPropertyName("played")]
    public DateTime? Played { get; init; }
    
    [XmlAttribute("userRating")]
    [JsonPropertyName("userRating")]
    public required int UserRating { get; init; }
    
    [XmlElement("musicBrainzId")]
    [JsonPropertyName("musicBrainzId")]
    public Guid? MusicBrainzId { get; init; }
    
    [XmlAttribute("displayArtist")]
    [JsonPropertyName("displayArtist")]
    public required string DisplayArtist { get; init; }
    
    [XmlAttribute("sortName")]
    [JsonPropertyName("sortName")]
    public required string SortName { get; init; }
    
    [XmlAttribute("isCompilation")]
    [JsonPropertyName("isCompilation")]
    public required bool IsCompilation { get; set; }
    
    [XmlElement("releaseTypes")]
    [JsonPropertyName("releaseTypes")]
    public required List<string> ReleaseTypes { get; set; }
    
    [XmlAttribute("explicitStatus")]
    [JsonPropertyName("explicitStatus")]
    public required string ExplicitStatus { get; init; }
    
    [XmlElement("artists")]
    [JsonPropertyName("artists")]
    public required List<NameIdEntity> Artists { get; set; }

    [XmlElement("song")]
    [JsonPropertyName("song")]
    public required List<TrackID3> Song { get; set; } = [];
    
    [XmlIgnore]
    [JsonIgnore]
    public required bool HasDuplicates { get; init; }
}