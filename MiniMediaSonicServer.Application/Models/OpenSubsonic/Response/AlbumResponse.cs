using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class AlbumResponse
{
    [XmlElement("album")]
    [JsonPropertyName("album")]
    public required AlbumID3 Album { get; init; }
}