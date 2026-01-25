using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class ReplayGain
{
    [XmlAttribute("trackGain")]
    [JsonPropertyName("trackGain")]
    public float TrackGain { get; set; }
    
    [XmlAttribute("albumGain")]
    [JsonPropertyName("albumGain")]
    public float AlbumGain { get; set; }
    
    [XmlAttribute("trackPeak")]
    [JsonPropertyName("trackPeak")]
    public float TrackPeak { get; set; }
    
    [XmlAttribute("albumPeak")]
    [JsonPropertyName("albumPeak")]
    public float AlbumPeak { get; set; }
    
    [XmlAttribute("baseGain")]
    [JsonPropertyName("baseGain")]
    public float BaseGain { get; set; }
    
    [XmlAttribute("fallbackGain")]
    [JsonPropertyName("fallbackGain")]
    public float FallbackGain { get; set; }
}