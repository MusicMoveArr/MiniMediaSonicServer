using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class AlbumListResponse
{
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public List<AlbumID3> Album { get; set; } = new();
}