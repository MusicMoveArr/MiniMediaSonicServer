using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class SimilarSongsList2Response
{
    [XmlElement("song")]
    [JsonPropertyName("song")]
    public List<TrackID3> Tracks { get; set; } = new();
}