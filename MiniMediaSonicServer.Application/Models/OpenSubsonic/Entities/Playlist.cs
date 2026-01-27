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

    [XmlAttribute("owner")]
    [JsonPropertyName("owner")]
    public string Owner { get; set; }

    [XmlIgnore]
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
    
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public List<TrackID3> Entry { get; set; }
}