using System.Text.Json.Serialization;
using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class SubsonicAuthModel
{
    [JsonPropertyName("u")]
    public string AuthUsername { get; set; }
    
    [JsonPropertyName("p")]
    public string AuthPassword { get; set; }
    
    [JsonPropertyName("t")]
    public string AuthToken { get; set; }
    
    [JsonPropertyName("s")]
    public string AuthSalt { get; set; }
    
    [JsonPropertyName("c")]
    public string AuthAppName { get; set; }
    
    [JsonPropertyName("f")]
    public string AuthOutputFormat { get; set; }
}