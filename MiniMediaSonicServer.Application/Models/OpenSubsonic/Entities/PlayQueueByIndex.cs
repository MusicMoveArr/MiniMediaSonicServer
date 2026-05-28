using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class PlayQueueByIndex
{
    [XmlAttribute("currentIndex")]
    [JsonPropertyName("currentIndex")]
    public int? CurrentIndex { get; set; }

    [XmlElement("position")]
    [JsonPropertyName("position")]
    public long? TrackPosition { get; set; }

    [XmlElement("username")]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [XmlElement("changed")]
    [JsonPropertyName("changed")]
    public DateTime? Changed { get; set; }

    [XmlElement("changedBy")]
    [JsonPropertyName("changedBy")]
    public string? ChangedBy { get; set; }

    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public List<TrackID3> Tracks { get; set; } = new List<TrackID3>();
}