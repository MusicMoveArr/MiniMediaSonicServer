using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class SubsonicAuthModel
{
    [JsonPropertyName("u")]
    public string Username { get; set; }
    
    [JsonPropertyName("p")]
    public string Password { get; set; }
    
    [JsonPropertyName("t")]
    public string Token { get; set; }
    
    [JsonPropertyName("s")]
    public string Salt { get; set; }
    
    [JsonPropertyName("c")]
    public string AppName { get; set; }
}