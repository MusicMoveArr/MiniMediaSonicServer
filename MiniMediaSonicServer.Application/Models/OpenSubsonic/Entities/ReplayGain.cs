using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class ReplayGain
{
    [XmlElement("trackGain")]
    [JsonPropertyName("trackGain")]
    public float? TrackGain { get; set; }
    
    [XmlElement("albumGain")]
    [JsonPropertyName("albumGain")]
    public float? AlbumGain { get; set; }
    
    [XmlElement("trackPeak")]
    [JsonPropertyName("trackPeak")]
    public float? TrackPeak { get; set; }
    
    [XmlElement("albumPeak")]
    [JsonPropertyName("albumPeak")]
    public float? AlbumPeak { get; set; }
    
    [XmlElement("baseGain")]
    [JsonPropertyName("baseGain")]
    public float? BaseGain { get; set; }
    
    [XmlElement("fallbackGain")]
    [JsonPropertyName("fallbackGain")]
    public float? FallbackGain { get; set; }
}