using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Playlist
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [XmlAttribute("comment")]
    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    [XmlAttribute("owner")]
    [JsonPropertyName("owner")]
    public string Owner { get; set; }

    [XmlElement("public")]
    [JsonPropertyName("public")]
    public bool? Public { get; set; }

    [XmlAttribute("songCount")]
    [JsonPropertyName("songCount")]
    public int SongCount { get; set; }

    [JsonPropertyName("duration")]
    [XmlAttribute("duration")]
    public int Duration { get; set; }

    [XmlAttribute("created")]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [XmlAttribute("changed")]
    [JsonPropertyName("changed")]
    public DateTime Changed { get; set; }

    [XmlAttribute("coverArt")]
    [JsonPropertyName("coverArt")]
    public string CoverArt { get; set; }

    [XmlElement("readonly")]
    [JsonPropertyName("readonly")]
    public bool ReadOnly { get; set; }

    [XmlElement("validUntil")]
    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil { get; set; }

    [XmlAttribute("allowedUser")]
    [JsonPropertyName("allowedUser")]
    public List<string> AllowedUser { get; set; }
    
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public List<TrackID3> Entry { get; set; }
}