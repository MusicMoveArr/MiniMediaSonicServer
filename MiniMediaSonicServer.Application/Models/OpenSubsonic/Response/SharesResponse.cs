using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class SharesResponse
{
    [XmlElement("share")]
    [JsonPropertyName("share")]
    public List<ShareResponse> Share { get; set; } = new();
}