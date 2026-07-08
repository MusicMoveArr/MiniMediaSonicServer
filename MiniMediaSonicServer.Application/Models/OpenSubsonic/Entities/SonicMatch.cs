using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class SonicMatch
{
    [XmlElement("entry")]
    [JsonPropertyName("entry")]
    public TrackID3 Song { get; set; }
    
    [XmlElement("similarity")]
    [JsonPropertyName("similarity")]
    public float Similarity { get; set; }
}