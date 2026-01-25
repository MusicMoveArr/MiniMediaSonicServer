using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic;

public sealed class License
{
    [XmlAttribute("valid")]
    [JsonPropertyName("valid")]
    public bool Valid { get; set; } = true;

    [XmlAttribute("email")]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [XmlAttribute("licenseExpires")]
    [JsonPropertyName("licenseExpires")]
    public string? LicenseExpires { get; set; }
}