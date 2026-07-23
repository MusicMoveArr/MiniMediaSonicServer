using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class ShareResponse
{
    [XmlElement("id")]
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }
    
    [XmlElement("url")]
    [JsonPropertyName("url")]
    public required string Url { get; init; }
    
    [XmlElement("description")]
    [JsonPropertyName("description")]
    public required string Description { get; init; }
    
    [XmlElement("username")]
    [JsonPropertyName("username")]
    public required string Username { get; init; }
    
    [XmlElement("created")]
    [JsonPropertyName("created")]
    public required DateTime Created { get; init; }
    
    [XmlElement("expires")]
    [JsonPropertyName("expires")]
    public required DateTime? Expires { get; init; }
    
    [XmlElement("lastVisited")]
    [JsonPropertyName("lastVisited")]
    public required DateTime? LastVisited { get; init; }
    
    [XmlElement("visitCount")]
    [JsonPropertyName("visitCount")]
    public required int VisitCount { get; init; }
    
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public required List<TrackID3> Tracks { get; init; } = [];
}