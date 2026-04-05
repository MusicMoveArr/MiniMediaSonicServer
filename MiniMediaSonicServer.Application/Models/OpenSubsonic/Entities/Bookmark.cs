using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Bookmark
{
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public TrackID3? Track { get; set; }
    
    [XmlAttribute("position")]
    [JsonPropertyName("position")]
    public long Position { get; set; }
    
    [XmlAttribute("username")]
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [XmlAttribute("comment")]
    [JsonPropertyName("comment")]
    public string Comment { get; set; }
    
    [XmlAttribute("created")]
    [JsonPropertyName("created")]
    public DateTime CreatedAt { get; set; }
    
    [XmlAttribute("changed")]
    [JsonPropertyName("changed")]
    public DateTime UpdatedAt { get; set; }
}