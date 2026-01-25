using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic;

public class Error
{
    [XmlAttribute("code")]
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [XmlAttribute("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}