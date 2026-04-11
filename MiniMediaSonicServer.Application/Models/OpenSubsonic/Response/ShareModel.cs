using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class ShareResponse
{
    [XmlElement("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [XmlElement("url")]
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [XmlElement("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [XmlElement("username")]
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [XmlElement("created")]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
    
    [XmlElement("expires")]
    [JsonPropertyName("expires")]
    public DateTime? Expires { get; set; }
    
    [XmlElement("lastVisited")]
    [JsonPropertyName("lastVisited")]
    public DateTime? LastVisited { get; set; }
    
    [XmlElement("visitCount")]
    [JsonPropertyName("visitCount")]
    public int VisitCount { get; set; }
    
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public List<TrackID3> Tracks { get; set; } = new();
}